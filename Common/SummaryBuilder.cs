using System.Web.UI.WebControls;

namespace ch.appl.psoft.Common
{
    public delegate void AddSummaryCellHandler(TableRow r, TableCell cell, int columnIndex, int indentLevel);
    /// <summary>
    /// Summary description for SummaryBuilder.
    /// </summary>
    public class SummaryBuilder
    {
        /// <summary>
        /// Add cell event. Called after addition of a cell.
        /// </summary>
        public event AddSummaryCellHandler addCell= null;

        protected const string _swapDisplay = "SwapDisplay";

        protected int _cellPadding = 4;
        protected int _cellSpacing = 1;
        protected int _buttonWidth = 20;
        protected int [] _columnWidths = null;
        protected HorizontalAlign [] _headerColumnAligns = null;
        protected HorizontalAlign [] _columnAligns = null;
        protected int _columnCount = 0;

        public int CellPadding{
            get {return _cellPadding;}
            set {_cellPadding =  value;}
        }

        public int CellSpacing{
            get {return _cellSpacing;}
            set {_cellSpacing =  value;}
        }

        public int ButtonWidth{
            get {return _buttonWidth;}
            set {_buttonWidth =  value;}
        }

        public int [] ColumnWidths{
            get {return _columnWidths;}
            set {_columnWidths =  value;}
        }

        public HorizontalAlign [] HeaderColumnAligns{
            get {return _headerColumnAligns;}
            set {_headerColumnAligns =  value;}
        }

        public HorizontalAlign [] ColumnAligns{
            get {return _columnAligns;}
            set {_columnAligns =  value;}
        }

        public int ColumnCount{
            get {return _columnCount;}
            set {_columnCount =  value;}
        }

        public SummaryBuilder(){
        }

        public SummaryBuilder(int cellPadding, int cellSpacing, int buttonWidth, int [] columnWidths, HorizontalAlign [] headerColumnAligns, HorizontalAlign [] columnAligns, int columnCount)
        {
            _cellPadding = cellPadding;
            _cellSpacing = cellSpacing;
            _buttonWidth = buttonWidth;
            _columnWidths = columnWidths;
            _headerColumnAligns = headerColumnAligns;
            _columnAligns = columnAligns;
            _columnCount = columnCount;
        }

        private string GetSwapDisplay(string element)
        {
            return _swapDisplay + "('" + element + "',this);";
        }

        protected void setLevelRowsCount(Table table, int rowsCount){
            string attr = table.Attributes["LevelRows"];
            if (attr == null){
                table.Attributes.Add("LevelRows", rowsCount.ToString()); 
            }
            else{
                table.Attributes["LevelRows"] = rowsCount.ToString();
            }
        }

        protected int getLevelRowsCount(Table table){
            return ch.psoft.Util.Validate.GetValid(table.Attributes["LevelRows"], 0);
        }

        public void BuildHeader(Table parentTable, string [] values)
        {
            TableRow r = new TableRow();
            r.CssClass = "ListHeader";
            parentTable.Rows.Add(r);
            setLevelRowsCount(parentTable, 0);

            TableHeaderCell c = new TableHeaderCell();
            c.Width = _buttonWidth;
            r.Cells.Add(c);

            for (int i=0; i<_columnCount; i++)
            {
                c = new TableHeaderCell();
                c.Style.Add("word-wrap","break-word");
                c.CssClass = "ListHeaderSummary";
                c.VerticalAlign = VerticalAlign.Bottom;
                if (i < _columnWidths.Length)
                {
                    c.Width = _columnWidths[i];
                }
                if (i < _headerColumnAligns.Length)
                {
                    c.HorizontalAlign = _headerColumnAligns[i];
                }
                if (i < values.Length)
                    c.Text = values[i];
                r.Cells.Add(c);
            }
        }

        public Table BuildChildTable(Table rootTable, string prefix, string recordID, int indentLevel)
        {
            Table parentTable = (Table) rootTable.FindControl("table" + prefix + recordID);
            if (parentTable != null)
                return parentTable;

            TableRow r = (TableRow) rootTable.FindControl("row" + prefix + recordID);
            
            parentTable = (Table) r.Parent;
            int rowIndex = parentTable.Rows.GetRowIndex(r);

            r = new TableRow();
            r.ID = prefix + recordID;
            r.Style.Add("padding","0");
            r.Style.Add("display","none");
            parentTable.Rows.AddAt(rowIndex + 1,r);

            TableCell c = new TableCell();
            c.ColumnSpan = _columnCount + indentLevel;
            c.Style.Add("padding-right","0");
            c.Style.Add("padding-left","0");
            r.Cells.Add(c);

            Table t = new Table();
            t.ID = "table" + prefix + recordID;
            t.CellSpacing = _cellSpacing;
            t.CellPadding = _cellPadding;
            t.BorderWidth = 0;
            setLevelRowsCount(t, 1);
            c.Controls.Add(t);

            c = (TableCell) r.FindControl("link" + prefix + recordID);
            HyperLink l = new HyperLink();
            l.Attributes.Add("href","#");
            l.Attributes.Add("onclick", GetSwapDisplay(r.ClientID));
            Image im = new Image();
            im.ImageUrl = Global.Config.baseURL + "/images/plus.gif";
            l.Controls.Add(im);
            c.Controls.Add(l);

            return t;
        }

        public void BuildEntry(Table parentTable, string prefix, string recordID, int indentLevel, string [] values)
        {
            TableRow r = new TableRow();
            r.ID = "row" + prefix + recordID;
            int levelRowsCount = getLevelRowsCount(parentTable);
            r.CssClass = levelRowsCount % 2 == 1 ? "ListEven" : "ListOdd";
            parentTable.Rows.Add(r);
            setLevelRowsCount(parentTable, levelRowsCount+1);

            TableCell c = null;
            for (int i=0; i<indentLevel-1; i++)
            {
                c = new TableCell();
                c.Width = _buttonWidth;
                c.CssClass = "ListOdd";
                r.Cells.Add(c);
            }

            c = new TableCell();
            c.ID = "link" + prefix + recordID;
            c.Width = _buttonWidth;
            r.Cells.Add(c);

            c = new TableCell();
            c.Width = _columnWidths[0] - (indentLevel - 1) * ((2 * _cellPadding) + _buttonWidth + _cellSpacing);
            c.Text = values[0];
            c.Style.Add("word-wrap","break-word");
            r.Cells.Add(c);
            onAddCell(r,c,0,indentLevel);

            for (int i=1; i<_columnCount; i++)
            {
                c = new TableCell();
                c.Style.Add("word-wrap","break-word");
                if (i < _columnWidths.Length)
                {
                    c.Width = _columnWidths[i];
                }
                if (i < _columnAligns.Length && HorizontalAlign.NotSet != _headerColumnAligns[i])
                {
                     c.Style.Add("text-align", _headerColumnAligns[i].ToString());
                }
                if (i < values.Length)
                    c.Text = values[i];
                r.Cells.Add(c);
                onAddCell(r,c,i,indentLevel);
            }

        }

        private void onAddCell(TableRow r, TableCell cell, int columnIndex, int indentLevel)
        {
            if (addCell != null) 
            {
                // Invokes the delegates. 
                addCell(r,cell,columnIndex,indentLevel);
            }
        }
    }
}

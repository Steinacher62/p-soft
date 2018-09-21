using ch.appl.psoft.Morph.lexer;
using iTextSharp.text;
using iTextSharp.text.rtf;
using System.Collections;
using System.Runtime.InteropServices;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Morph
{
    public class TableToWord
    {
        // Fields
        private int _headingLevel;
        private System.Web.UI.WebControls.Table _table;
        private iTextSharp.text.Paragraph _title;
        private RtfWriter2 _writer;
        private ArrayList proportionPairList;
        private int totalPercent;

        // Methods
        public TableToWord(System.Web.UI.WebControls.Table table, RtfWriter2 writer)
            : this(table, writer, 1)
        {
        }

        public TableToWord(System.Web.UI.WebControls.Table table, RtfWriter2 writer, int headingLevel)
        {
            this._headingLevel = 1;
            this.proportionPairList = new ArrayList();
            this._table = table;
            this._writer = writer;
            this._headingLevel = headingLevel;
        }

        public bool addColumnProportion(int index, int percent)
        {
            if (this._table.Rows.Count > 0)
            {
                int num = this._table.Rows[0].Cells.Count - 1;
                if (((index < num) && (percent < 100)) && (percent > 0))
                {
                    this.proportionPairList.Add(new ProportionPair(index, percent));
                    this.totalPercent += percent;
                    return true;
                }
            }
            return false;
        }

        public void printTable()
        {
            iTextSharp.text.Table element = null;
            int count = this._table.Rows.Count;
            int num2 = 0;
            if ((count > 0) && (this._table.Rows[0] != null))
            {
                num2 = this._table.Rows[0].Cells.Count;
                if (num2 > 1)
                {
                    element = new iTextSharp.text.Table(num2 - 1);
                    element.CellsFitPage = true;
                    element.Alignment = 0;
                    element.Cellpadding = 5f;
                    this._writer.SetMargins(24f, 24f, 24f, 24f);
                    element.WidthPercentage = 90f;
                    element.TableFitsPage = true;
                    element.UseVariableBorders = true;
                    for (int i = 0; i < count; i++)
                    {
                        for (int k = 1; k < num2; k++)
                        {
                            string input = "";
                            if (this._table.Rows[i].Cells[k] is TableHeaderCell)
                            {
                                for (int m = 0; m < this._table.Rows[i].Cells[k].Controls.Count; m++)
                                {
                                    if (this._table.Rows[i].Cells[k].Controls[m] is HyperLink)
                                    {
                                        input = input + ((HyperLink)this._table.Rows[i].Cells[k].Controls[m]).Text;
                                    }
                                }
                            }
                            else
                            {
                                input = this._table.Rows[i].Cells[k].Text;
                            }
                            input = HtmlToCharMapper.convert(input);
                            Chunk chunk = new Chunk(input, MorphToWord.STYLE_NORMAL);
                            if (i == 0)
                            {
                                chunk = new Chunk(input, MorphToWord.STYLE_BOLD);
                            }
                            Cell cell = new Cell(chunk);
                            cell.GrayFill = 10f;
                            element.AddCell(cell);
                        }
                    }
                    int num6 = num2 - 1;
                    int num7 = (100 - this.totalPercent) / (num6 - this.proportionPairList.Count);
                    int[] widths = new int[num6];
                    for (int j = 0; j < num6; j++)
                    {
                        widths[j] = num7;
                    }
                    foreach (ProportionPair pair in this.proportionPairList)
                    {
                        widths[pair.index] = pair.percent;
                    }
                    element.SetWidths(widths);
                    this._writer.Add(this._title);
                    this._writer.Add(new Paragraph("", MorphToWord.STYLE_NORMAL));
                    this._writer.Add(element);
                    this._writer.Add(new Paragraph("", MorphToWord.STYLE_NORMAL));
                }
            }
        }




        public TableToWord printTitle(string title)
        {
            switch (this._headingLevel)
            {
                case 1:
                    this._title = new iTextSharp.text.Paragraph(title, MorphToWord.STYLE_HEADING_1);
                    break;

                case 2:
                    this._title = new iTextSharp.text.Paragraph(title, MorphToWord.STYLE_HEADING_2);
                    break;

                case 3:
                    this._title = new iTextSharp.text.Paragraph(title, MorphToWord.STYLE_HEADING_3);
                    break;

                default:
                    this._title = new iTextSharp.text.Paragraph(title, MorphToWord.STYLE_HEADING_1);
                    break;
            }
            return this;
        }

        // Nested Types
        [StructLayout(LayoutKind.Sequential)]
        public struct ProportionPair
        {
            public int index;
            public int percent;
            public ProportionPair(int index, int percent)
            {
                this.index = index;
                this.percent = percent;
            }
        }
    }
}
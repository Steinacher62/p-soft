using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.appl.psoft.Morph.lexer;
using ch.psoft.Util;
using iTextSharp.text;
using iTextSharp.text.rtf;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.field;
using iTextSharp.text.rtf.style;
using System.Collections;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;

namespace ch.appl.psoft.Morph
{
    public class MorphToWord
    {
        // Fields
        private LanguageMapper _mapper;
        private DBData db;
        public const int DEFAULT_FONT_SIZE = 12;
        public const string DEFAULT_FONT_STYLE = "Arial";
        private iTextSharp.text.Document document = new iTextSharp.text.Document();
        private const string INDEX_OF_CONTENTS = "{Inhaltsverzeichnis: Um zu aktualisieren, rechte Maustaste hier klicken}";
        public static int MAXHEADING = 8;
        private long owner_uid;
        private const string RANKING_SELECTED_SYMBOL = " X";
        public static RtfDocument rtfdocument = new RtfDocument();
        public static RtfParagraphStyle STYLE_BOLD = new RtfParagraphStyle("BOLD", "Arial", 12, STYLE_NORMAL.Style, STYLE_NORMAL.Color);
        public static RtfParagraphStyle STYLE_BOLDITALIC = new RtfParagraphStyle("BOLDITALIC", "Arial", 12, STYLE_NORMAL.Style, STYLE_NORMAL.Color);
        public static RtfParagraphStyle STYLE_HEADING_1 = RtfParagraphStyle.STYLE_HEADING_1;
        public static RtfParagraphStyle STYLE_HEADING_2 = RtfParagraphStyle.STYLE_HEADING_2;
        public static RtfParagraphStyle STYLE_HEADING_3 = RtfParagraphStyle.STYLE_HEADING_3;
        public static RtfParagraphStyle[] STYLE_HEADING_ARRAY = new RtfParagraphStyle[MAXHEADING - 3];
        public static RtfParagraphStyle STYLE_ITALIC = new RtfParagraphStyle("ITALIC", "Arial", 12, STYLE_NORMAL.Style, STYLE_NORMAL.Color);
        public static RtfParagraphStyle STYLE_NORMAL = RtfParagraphStyle.STYLE_NORMAL;
        public static RtfParagraphStyle STYLE_UNDERLINE = new RtfParagraphStyle("UNDERLINE", "Arial", 12, STYLE_NORMAL.Style, STYLE_NORMAL.Color);
        public static RtfParagraphStyle STYLE_URL = new RtfParagraphStyle("URL", "Arial", 12, STYLE_NORMAL.Style, STYLE_NORMAL.Color);
        private RtfWriter2 writer;

        // Methods
        static MorphToWord()
        {
            STYLE_NORMAL.SetFontName("Arial");
            STYLE_NORMAL.Size = 12f;
            STYLE_HEADING_1.SetFontName("Arial");
            STYLE_HEADING_2.SetFontName("Arial");
            STYLE_HEADING_2.SetStyle(-2);
            STYLE_HEADING_3.SetFontName("Arial");
            STYLE_BOLD.SetStyle(1);
            STYLE_ITALIC.SetStyle(2);
            STYLE_BOLDITALIC.SetStyle(3);
            STYLE_UNDERLINE.SetStyle(4);
            STYLE_URL.SetStyle(4);
            STYLE_URL.SetColor(0, 0, 0xff);
        }

        public MorphToWord(string filename, DBData db, long owner_uid)
        {
            this.db = db;
            this.owner_uid = owner_uid;
            this.document.AddTitle("");
            this.document.AddSubject("");
            this.document.AddKeywords("");
            this.document.AddCreator("p-soft");
            string author = db.Person.getWholeName(owner_uid);
            this.document.AddAuthor(author);
            this.document.AddCreationDate();
            this.document.AddHeader("\namespace", "\test");
            this.writer = RtfWriter2.GetInstance(this.document, new FileStream(filename, FileMode.OpenOrCreate));
            for (int i = 0; i < STYLE_HEADING_ARRAY.Length; i++)
            {
                int num2 = i + 4;
                RtfParagraphStyle style = new RtfParagraphStyle("heading " + num2, "Normal");
                style.SetFontName("Arial");
                STYLE_HEADING_ARRAY[i] = style;
                this.writer.GetDocumentSettings().RegisterParagraphStyle(STYLE_HEADING_ARRAY[i]);
            }
            this.writer.GetDocumentSettings().RegisterParagraphStyle(STYLE_BOLD);
            this.writer.GetDocumentSettings().RegisterParagraphStyle(STYLE_ITALIC);
            this.writer.GetDocumentSettings().RegisterParagraphStyle(STYLE_BOLDITALIC);
            this.writer.GetDocumentSettings().RegisterParagraphStyle(STYLE_UNDERLINE);
            this.writer.GetDocumentSettings().RegisterParagraphStyle(STYLE_URL);
            this.writer.SetAutogenerateTOCEntries(true);
            this.document.Open();
        }

        public void close()
        {
            this.document.Close();
        }

        public static string outputFile(string fileName, string printTargetDirectory)
        {
            return (Global.Config.documentSaveDirectory + @"\" + printTargetDirectory + @"\" + fileName);
        }

        public void pageBreak()
        {
            this.writer.NewPage();
        }

        public static TitleWikiPair prepareTheme(DBData db, int id)
        {
            string selectStatement = "select * from THEME where ID = " + id;
            DataTable table = db.getDataTable(selectStatement, new object[] { Logger.VERBOSE });
            if (table.Rows.Count > 0)
            {
                string title = (table.Rows[0][6] is string) ? ((string)table.Rows[0][6]) : "";
                return new TitleWikiPair(title, (table.Rows[0][7] is string) ? ((string)table.Rows[0][7]) : "");
            }
            return new TitleWikiPair("", "");
        }

        public static TitleWikiPair[] prepareThemen(DBData db, long knowledgeID)
        {
            int count = 0;
            long num2 = db.Knowledge.getBaseThemeID(knowledgeID);
            string selectStatement = "select * from THEME where ROOT_ID = " + num2;
            DataTable table = db.getDataTable(selectStatement, new object[] { Logger.VERBOSE });
            count = table.Rows.Count;
            TitleWikiPair[] pairArray = new TitleWikiPair[count - 1];
            for (int i = 1; i < count; i++)
            {
                string title = (table.Rows[i][6] is string) ? ((string)table.Rows[i][6]) : "";
                string wikiText = (table.Rows[i][7] is string) ? ((string)table.Rows[i][7]) : "";
                pairArray[i - 1] = new TitleWikiPair(title, wikiText);
            }
            return pairArray;
        }

        public void printAllThemen(TitleWikiPair[] pairs)
        {
            for (int i = 0; i < pairs.Length; i++)
            {
                this.printWiki(pairs[i], 2);
            }
        }

        public void printIndex()
        {
            this.document.Add(new Paragraph());
            Paragraph element = new Paragraph();
            element.Add(new RtfTableOfContents("{Inhaltsverzeichnis: Um zu aktualisieren, rechte Maustaste hier klicken}"));
            this.document.Add(element);
            this.document.Add(new Paragraph());
        }

        public void printNewLine()
        {
            this.document.Add(new Paragraph("\n", STYLE_NORMAL));
        }

        public void printParagraph(Paragraph p)
        {
            this.writer.Add(p);
        }

        public void printRanking(string title, string remark, ArrayList ranklist, int headingLevel)
        {
            if (this.document.IsOpen())
            {
                switch (headingLevel)
                {
                    case 1:
                        this.document.Add(new Paragraph(title, STYLE_HEADING_1));
                        break;

                    case 2:
                        this.document.Add(new Paragraph(title, STYLE_HEADING_2));
                        break;

                    case 3:
                        this.document.Add(new Paragraph(title, STYLE_HEADING_3));
                        break;

                    default:
                        this.document.Add(new Paragraph(title, STYLE_HEADING_1));
                        break;
                }
                iTextSharp.text.Table element = new iTextSharp.text.Table(3, ranklist.Count);
                element.Width = 80f;
                element.Alignment = 0;
                element.Cellpadding = 5f;
                this.document.SetMargins(24f, 24f, 24f, 24f);
                string newValue = "";
                foreach (RankingField field in ranklist)
                {
                    string content = "";
                    if (field.selected)
                    {
                        content = " X";
                    }
                    Cell cell = new Cell(new Chunk(content, STYLE_NORMAL));
                    Cell cell2 = new Cell(new Chunk("", STYLE_NORMAL));
                    Cell cell3 = new Cell(new Chunk(field.text, STYLE_NORMAL));
                    if (field.selected)
                    {
                        cell3.BackgroundColor = new Color(System.Drawing.Color.LightGray);
                        cell.BackgroundColor = new Color(System.Drawing.Color.LightGray);
                        newValue = field.text;
                    }
                    cell2.BackgroundColor = new Color(field.color);
                    element.AddCell(cell);
                    element.AddCell(cell2);
                    element.AddCell(cell3);
                }
                int[] widths = new int[] { 5, 5, 70 };
                element.SetWidths(widths);
                if (this._mapper != null)
                {
                    newValue = this._mapper.get("morph", "ptSelected").Replace("#1", newValue);
                    remark = this._mapper.get("morph", "ptRemark").Replace("#1", remark);
                }
                else
                {
                    newValue = "Selection: " + newValue;
                }
                this.document.Add(new Paragraph("", STYLE_NORMAL));
                this.document.Add(element);
                this.document.Add(new Paragraph("", STYLE_NORMAL));
                this.document.Add(new Paragraph(remark, STYLE_NORMAL));
            }
        }

        public void printRegistratur(string title, string[] regs)
        {
            this.printRegistratur(title, regs, 1);
        }

        public void printRegistratur(string title, string[] regs, int headingLevel)
        {
            if (regs.Length > 0)
            {
                switch (headingLevel)
                {
                    case 1:
                        this.document.Add(new Paragraph(title, STYLE_HEADING_1));
                        break;

                    case 2:
                        this.document.Add(new Paragraph(title, STYLE_HEADING_2));
                        break;

                    default:
                        this.document.Add(new Paragraph(title, STYLE_HEADING_1));
                        break;
                }
                this.document.Add(new Paragraph("", STYLE_NORMAL));
                RtfParagraphStyle style1 = STYLE_NORMAL;
                List element = new List(false, 20f);
                for (int i = 0; i < regs.Length; i++)
                {
                    element.Add(new iTextSharp.text.ListItem(regs[i], STYLE_NORMAL));
                }
                this.document.Add(element);
            }
        }

        public void printTable(string title, System.Web.UI.WebControls.Table table)
        {
            this.printTable(title, table, 1);
        }

        public void printTable(string title, System.Web.UI.WebControls.Table table, int headingLevel)
        {
            TableToWord word = new TableToWord(table, this.writer, headingLevel);
            word.printTitle(title).printTable();
        }

        public void printTable(string title, System.Web.UI.WebControls.Table table, TableToWord.ProportionPair[] proportionPairArray)
        {
            TableToWord word = new TableToWord(table, this.writer, 1);
            for (int i = 0; (proportionPairArray != null) && (i < proportionPairArray.Length); i++)
            {
                word.addColumnProportion(proportionPairArray[i].index, proportionPairArray[i].percent);
            }
            word.printTitle(title).printTable();
        }

        public void printTable(string title, System.Web.UI.WebControls.Table table, int headingLevel, TableToWord.ProportionPair[] proportionPairArray)
        {
            TableToWord word = new TableToWord(table, this.writer, headingLevel);
            for (int i = 0; (proportionPairArray != null) && (i < proportionPairArray.Length); i++)
            {
                word.addColumnProportion(proportionPairArray[i].index, proportionPairArray[i].percent);
            }
            word.printTitle(title).printTable();
        }

        public void printWiki(TitleWikiPair pair, int startHeadingLevel)
        {
            switch (startHeadingLevel)
            {
                case 1:
                    this.document.Add(new Paragraph(pair.title, STYLE_HEADING_1));
                    break;

                case 2:
                    {
                        Paragraph element = new Paragraph(pair.title, STYLE_HEADING_2);
                        this.document.Add(element);
                        break;
                    }
                case 3:
                    this.document.Add(new Paragraph(pair.title, STYLE_HEADING_3));
                    break;

                default:
                    this.document.Add(new Paragraph(pair.title, STYLE_HEADING_1));
                    break;
            }
            new WikiToWord(pair.wikiText + "\n", this.writer, startHeadingLevel, this.db, this.owner_uid).print();
        }

        public void printWissenElement(TitleWikiPair pair)
        {
            this.document.Add(new Paragraph(pair.title, STYLE_HEADING_1));
            int startingHeadingLevel = 1;
            new WikiToWord(pair.wikiText + "\n", this.writer, startingHeadingLevel, this.db, this.owner_uid).print();
        }

        // Properties
        public DBData Db
        {
            set
            {
                this.db = value;
            }
        }

        public LanguageMapper Mapper
        {
            set
            {
                this._mapper = value;
            }
        }

        public long OwnerId
        {
            set
            {
                this.owner_uid = value;
            }
        }

        // Nested Types
        [StructLayout(LayoutKind.Sequential)]
        public struct RankingField
        {
            public bool selected;
            public System.Drawing.Color color;
            public string text;
            public RankingField(bool selected, System.Drawing.Color color, string text)
            {
                this.selected = selected;
                this.color = color;
                this.text = text;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TitleWikiPair
        {
            public string title;
            public string wikiText;
            public TitleWikiPair(string title, string wikiText)
            {
                this.title = title;
                this.wikiText = wikiText;
            }
        }
    }
}
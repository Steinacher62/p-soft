using ch.appl.psoft.db;
using ch.psoft.Util;
using iTextSharp.text;
using iTextSharp.text.rtf;
using iTextSharp.text.rtf.direct;
using System;
using System.Collections;
using System.IO;

namespace ch.appl.psoft.Morph.lexer
{
    internal class WikiToWord
    {
        // Fields
        //public int currentLevel;
        private DBData db;
        private bool fillingList;
        private ListManager liManager = new ListManager();
        private Line line = new Line();
        private Line.ParagraphType nextParagraph;
        private long owner_uid;
        private int startingHeadingLevel;
        private const int SYMBOL_IDENT = 10;
        private string wikiText;
        private RtfWriter2 writer;

        // Methods
        public WikiToWord(string wikiText, RtfWriter2 writer, int startingHeadingLevel, DBData db, long owner_uid)
        {
            this.writer = writer;
            this.wikiText = wikiText;
            this.startingHeadingLevel = startingHeadingLevel;
            this.db = db;
            this.owner_uid = owner_uid;
        }

        private void eof()
        {
            if (!this.liManager.empty())
            {
                this.liManager.print(this.writer);
                this.liManager.clear();
            }
            this.writer.Add(this.line.print(this.nextParagraph));
        }

        private void parse(Yytoken currentToken)
        {
            switch (currentToken.SymbolId)
            {
                case 1:
                    {
                        Interpreter.Image image = new Interpreter.Image(currentToken.Value.ToString(), 7, 2, this.db, this.owner_uid);
                        try
                        {
                            this.line.appendImage(image.Path, image.Description, image.isThumb(), image.isRight());
                        }
                        catch (Exception exception)
                        {
                            Logger.Log(exception, Logger.ERROR);
                        }
                        return;
                    }
                case 2:
                case 3:
                    {
                        this.fillingList = true;
                        int level = (int)currentToken.Value;
                        this.liManager.synchlist(level, currentToken.SymbolId == 2);
                        return;
                    }
                case 4:
                    {
                        int num2 = (int)currentToken.Value;
                        char[] chArray = new char[num2];
                        for (int i = 0; i < num2; i++)
                        {
                            chArray[i] = '\t';
                        }
                        this.line.appendText(new string(chArray), MorphToWord.STYLE_NORMAL);
                        return;
                    }
                case 5:
                    this.line.setBold();
                    return;

                case 6:
                    this.line.setItalic();
                    return;

                case 7:
                    this.line.setBoldItalic();
                    return;

                case 8:
                    this.line.appendEOL();
                    if (this.fillingList)
                    {
                        this.liManager.appendToCurrent(this.line);
                        this.fillingList = false;
                        break;
                    }
                    if (!this.liManager.empty())
                    {
                        this.liManager.print(this.writer);
                        this.liManager.clear();
                    }
                    this.writer.Add(this.line.print(this.nextParagraph));
                    this.nextParagraph = Line.ParagraphType.NORMAL;
                    break;

                case 9:
                    this.line.appendAny(currentToken.Value.ToString());
                    return;

                case 10:
                    this.line.appendText(currentToken.Value.ToString());
                    return;

                case 11:
                    this.line.setUnderlined();
                    return;

                case 12:
                    {
                        if (!this.liManager.empty())
                        {
                            if (!this.liManager.empty())
                            {
                                this.liManager.print(this.writer);
                                this.liManager.clear();
                            }
                            this.writer.Add(this.line.print());
                            this.line = new Line();
                            this.fillingList = false;
                        }
                        Paragraph element = ((HeadingManager)currentToken.Value).formatParagraph(this.startingHeadingLevel);
                        this.writer.Add(element);
                        return;
                    }
                case 13:
                    this.line.appendURL(currentToken.Value.ToString());
                    return;

                case 14:
                    {
                        Interpreter.UrlInterpreter interpreter = new Interpreter.UrlInterpreter(currentToken.Value.ToString(), 1, 1);
                        string url = interpreter.Url;
                        string description = interpreter.Description;
                        this.line.appendURL(url, description);
                        return;
                    }
                case 15:
                    this.line.appendEmail(currentToken.Value.ToString());
                    return;

                case 0x10:
                    this.line.appendHorizontalLine(this.writer);
                    return;

                case 0x11:
                    this.nextParagraph = Line.ParagraphType.RIGHT;
                    return;

                case 0x12:
                    this.nextParagraph = Line.ParagraphType.CENTERED;
                    return;

                case 0x13:
                    this.nextParagraph = Line.ParagraphType.BLOCK;
                    return;

                case 20:
                    {
                        if (currentToken.Value.ToString().IndexOf("|") > 0)
                        {
                            Interpreter.InternalLink link = new Interpreter.InternalLink(currentToken.Value.ToString(), 6, 7, this.db, this.owner_uid);
                            this.line.appendInternalLink(link.NiceName);
                            return;
                        }
                        Interpreter.InternalLink link2 = new Interpreter.InternalLink(currentToken.Value.ToString(), 6, 2, this.db, this.owner_uid);
                        this.line.appendInternalLink(link2.NiceName);
                        return;
                    }
                default:
                    Console.WriteLine("Warning, unknown token:" + currentToken.Value);
                    this.line.appendUnknown(currentToken.Value.ToString());
                    return;
            }
            this.line = new Line();
        }

        public void print()
        {
            if (this.wikiText.Length != 0)
            {
                StringReader @in = new StringReader(this.wikiText);
                Lexer lexer = null;
                try
                {
                    lexer = new Lexer(@in);
                    while (!lexer.isZzAtEOF())
                    {
                        Yytoken currentToken = lexer.yylex();
                        if (currentToken == null)
                        {
                            this.eof();
                            return;
                        }
                        this.parse(currentToken);
                    }
                }
                catch (IOException exception)
                {
                    Console.WriteLine("IO error scanning text \"{0}\"");
                    Console.WriteLine(exception);
                }
                catch (Exception exception2)
                {
                    Console.WriteLine("Unexpected exception:");
                    Console.WriteLine(exception2.ToString());
                }
            }
        }

        // Nested Types
        private class Line
        {
            // Fields
            private Font font = MorphToWord.STYLE_NORMAL;
            private string line = "";
            private Paragraph paragraph = new Paragraph();
            private Font prev = MorphToWord.STYLE_NORMAL;
            private bool stateBold;
            private bool stateBoldItalic;
            private bool stateItalic;
            private bool stateUnderlined;

            // Methods
            public void appendAny(string str)
            {
                this.line = this.line + str;
                this.paragraph.Add(new Chunk(str, MorphToWord.STYLE_NORMAL));
            }

            internal void appendEmail(string p)
            {
                Font font = MorphToWord.STYLE_URL;
                Anchor o = new Anchor(new Chunk(p, font));
                o.Reference = "mailto://" + p;
                o.Name = p;
                this.paragraph.Add(o);
            }

            public void appendEOL()
            {
                this.line = this.line + "\n";
            }

            internal void appendHorizontalLine(RtfWriter2 writer)
            {
                TextReader documentSource = new StringReader(@"\pard\plain\s0\fi0\li0\ri0\sl320\plain\f0 \li0\ri0\widctlpar\brdrb\brdrs\brdrw15\brsp20 \par \b0\par");
                RtfImportMappings mappings = new RtfImportMappings();
                writer.ImportRtfFragment(documentSource, mappings);
            }

            internal void appendImage(string p, string description, bool thumb, bool right)
            {
                try
                {
                    Image instance = Image.GetInstance(p);
                    if (thumb)
                    {
                        instance.Annotation = new Annotation(0f, 0f, 0f, 0f, description);
                    }
                    if (right)
                    {
                        instance.Alignment = 2;
                    }
                    this.paragraph.Add(instance);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception, Logger.DEBUG);
                }
            }

            internal void appendInternalLink(string p)
            {
                this.appendText(p);
            }

            internal void appendText(string p)
            {
                this.line = this.line + p;
                Chunk o = new Chunk(p, this.font);
                this.paragraph.Add(o);
            }

            internal void appendText(string p, Font f)
            {
                this.line = this.line + p;
                Chunk o = new Chunk(p, f);
                this.paragraph.Add(o);
            }

            internal void appendUnknown(string p)
            {
                this.paragraph.Add("unknown<" + p + ">");
            }

            internal void appendURL(string p)
            {
                this.appendURL(p, p);
            }

            internal void appendURL(string p, string description)
            {
                Font font = MorphToWord.STYLE_URL;
                Anchor o = new Anchor(new Chunk(description, font));
                if (p.IndexOf("http://") != 0)
                {
                    o.Reference = "http://" + p;
                }
                else
                {
                    o.Reference = p;
                }
                o.Name = description;
                this.paragraph.Add(o);
            }

            public void clear()
            {
                this.line = "";
                this.paragraph.Clear();
            }

            internal Phrase print()
            {
                return this.print(ParagraphType.NORMAL);
            }

            internal Phrase print(ParagraphType paragraphType)
            {
                Console.Write(this.line);
                Console.Write(this.paragraph.Count);
                switch (paragraphType)
                {
                    case ParagraphType.CENTERED:
                        this.paragraph.Alignment = 1;
                        break;

                    case ParagraphType.RIGHT:
                        this.paragraph.Alignment = 2;
                        break;

                    case ParagraphType.BLOCK:
                        this.paragraph.Alignment = 3;
                        break;
                }
                return this.paragraph;
            }

            internal void setBold()
            {
                this.stateBold = this.setFont(this.stateBold, 1);
            }

            internal void setBoldItalic()
            {
                this.stateBoldItalic = this.setFont(this.stateBoldItalic, 3);
            }

            internal bool setFont(bool isset, int type)
            {
                if (isset)
                {
                    this.font = this.prev;
                    return false;
                }
                this.prev = this.font;
                switch (type)
                {
                    case 1:
                        this.font = MorphToWord.STYLE_BOLD;
                        break;

                    case 2:
                        this.font = MorphToWord.STYLE_ITALIC;
                        break;

                    case 3:
                        this.font = MorphToWord.STYLE_BOLDITALIC;
                        break;

                    case 4:
                        this.font = MorphToWord.STYLE_UNDERLINE;
                        break;

                    default:
                        this.font = MorphToWord.STYLE_NORMAL;
                        break;
                }
                return true;
            }

            internal void setItalic()
            {
                this.stateItalic = this.setFont(this.stateItalic, 2);
            }

            internal void setNormal()
            {
                this.setFont(false, -1);
            }

            internal void setUnderlined()
            {
                this.stateUnderlined = this.setFont(this.stateUnderlined, 4);
            }

            // Properties
            public string CurLine
            {
                get
                {
                    return this.line;
                }
            }

            public Paragraph CurParagraph
            {
                get
                {
                    return this.paragraph;
                }
            }

            // Nested Types
            public enum ParagraphType
            {
                NORMAL,
                CENTERED,
                RIGHT,
                BLOCK
            }
        }

        private class ListManager
        {
            // Fields
            private int currentLevel;
            private Stack liStack = new Stack();
            private List root;

            // Methods
            private void addToCurrent(List list)
            {
                ((List)this.liStack.Peek()).Add(list);
            }

            public void appendToCurrent(WikiToWord.Line line)
            {
                ((List)this.liStack.Peek()).Add(new ListItem(line.CurParagraph));
                Console.WriteLine(string.Concat(new object[] { "add to: ", ((List)this.liStack.Peek()).Size, " ", this.currentLevel }));
            }

            internal void clear()
            {
                this.root = null;
                this.liStack.Clear();
                this.currentLevel = 0;
            }

            internal bool empty()
            {
                return (this.currentLevel == 0);
            }

            internal void print(RtfWriter2 doc)
            {
                for (int i = 0; i < this.root.Size; i++)
                {
                    Console.WriteLine(this.root.ToString());
                }
                doc.Add(this.root);
            }

            public void synchlist(int level, bool isNumbered)
            {
                if (level > this.currentLevel)
                {
                    for (int i = this.currentLevel; i < level; i++)
                    {
                        List list = new List(isNumbered, 10f);
                        if ((this.liStack != null) && (this.liStack.Count != 0))
                        {
                            this.addToCurrent(list);
                        }
                        else
                        {
                            this.root = list;
                        }
                        this.liStack.Push(list);
                    }
                }
                else if (level != this.currentLevel)
                {
                    for (int j = level; j < this.currentLevel; j++)
                    {
                        this.liStack.Pop();
                    }
                }
                Console.WriteLine(string.Concat(new object[] { "Level: ", level, " (", this.currentLevel, ") ", this.root.Size, " stack: ", this.liStack.Count }));
                if (this.liStack.Peek() == this.root)
                {
                    Console.WriteLine("equal");
                }
                this.currentLevel = level;
            }
        }
    }
}

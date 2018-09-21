using ch.appl.psoft.db;
using ch.psoft.Util;
using iTextSharp.text;
using iTextSharp.text.rtf;
using System;
using System.Collections;
using System.IO;
using System.Reflection;

namespace ch.appl.psoft.Morph
{
    public class MorphTextReport
    {
        // Fields
        public static object oEndOfDoc = @"\endofdoc";
        public static object oFalse = false;
        public static object oMissing = Missing.Value;
        public static object oTrue = true;

        // Methods
        public void create(DBData db, long OwnerUID, FileStream outfile, string wikiText)
        {
            iTextSharp.text.Document document = new iTextSharp.text.Document();
            try
            {
                RtfWriter.GetInstance(document, outfile);
                document.AddTitle("");
                document.AddSubject("");
                document.AddKeywords("");
                document.AddCreator("p-soft");
                document.AddAuthor("");
                document.Open();
                ParagraphFormat selection = new ParagraphFormat(document);
                new Wiki2WordProcessor(db).run(OwnerUID, selection, wikiText);
            }
            catch (Exception exception)
            {
                Logger.Log(exception, Logger.ERROR);
            }
            finally
            {
                document.Close();
            }
        }

        public static FileStream outputFile(string fileName)
        {
            string text1 = Global.Config.documentSaveDirectory + @"\" + fileName;
            string str = fileName.Replace(".htm", ".doc");
            return new FileStream(Global.Config.documentSaveDirectory + @"\x\" + str, FileMode.Create);
        }

        // Nested Types
        public class BoldToken : MorphTextReport.WikiToken
        {
            // Methods
            public override bool match(MorphTextReport.Wiki2WordProcessor p, string s)
            {
                return ((s != null) && s.StartsWith("'''"));
            }

            public override int process(string s, MorphTextReport.Wiki2WordProcessor p)
            {
                p.Selection.Font.Bold = true;
                return 3;
            }
        }

        public class HeadingToken : MorphTextReport.WikiToken
        {
            // Methods
            private int getHeadingSize(string s)
            {
                int num = 1;
                while (s[num - 1] == '=')
                {
                    num++;
                }
                return (num - 1);
            }

            public override bool match(MorphTextReport.Wiki2WordProcessor p, string s)
            {
                return ((s != null) && s.StartsWith("="));
            }

            private void OnLineBreak(MorphTextReport.Wiki2WordProcessor p)
            {
                p.Selection.set_Style_Standard();
                p.LineBreak -= new MorphTextReport.Wiki2WordProcessor.LineBreakEventHandler(this.OnLineBreak);
            }

            public override int process(string s, MorphTextReport.Wiki2WordProcessor p)
            {
                int headingSize = this.getHeadingSize(s);
                p.Selection.set_Style_Heading(headingSize);
                p.LineBreak += new MorphTextReport.Wiki2WordProcessor.LineBreakEventHandler(this.OnLineBreak);
                return headingSize;
            }
        }

        public class IndentToken : MorphTextReport.WikiToken
        {
            // Fields
            private int currentIndentLevel;

            // Methods
            private int getIndentLevel(string s)
            {
                int num = 1;
                while (s[num - 1] == ':')
                {
                    num++;
                }
                return num;
            }

            public override bool match(MorphTextReport.Wiki2WordProcessor p, string s)
            {
                char c = ' ';
                if (p.ProcessedString.Length > 0)
                {
                    c = p.ProcessedString[p.ProcessedString.Length - 1];
                }
                return (((s != null) && char.IsWhiteSpace(c)) && (s.StartsWith(":") && (s.TrimEnd(": ".ToCharArray()).Length > 0)));
            }

            public override int process(string s, MorphTextReport.Wiki2WordProcessor p)
            {
                int num = this.getIndentLevel(s);
                if (num > this.currentIndentLevel)
                {
                    for (int j = this.currentIndentLevel; j < num; j++)
                    {
                        p.Selection.Paragraphs.Indent();
                    }
                    return num;
                }
                for (int i = num; i > this.currentIndentLevel; i--)
                {
                    p.Selection.Paragraphs.Outdent();
                }
                return num;
            }
        }

        public class ItalicBoldToken : MorphTextReport.WikiToken
        {
            // Methods
            public override bool match(MorphTextReport.Wiki2WordProcessor p, string s)
            {
                return ((s != null) && s.StartsWith("'''''"));
            }

            public override int process(string s, MorphTextReport.Wiki2WordProcessor p)
            {
                p.Selection.Font.Italic = true;
                p.Selection.Font.Bold = true;
                return 5;
            }
        }

        public class ItalicToken : MorphTextReport.WikiToken
        {
            // Fields
            private bool started;

            // Methods
            public override bool match(MorphTextReport.Wiki2WordProcessor p, string s)
            {
                if ((s == null) || !s.StartsWith("''"))
                {
                    return false;
                }
                if (!this.started)
                {
                    this.started = true;
                    return true;
                }
                this.started = false;
                return true;
            }

            public override int process(string s, MorphTextReport.Wiki2WordProcessor p)
            {
                p.Selection.Font.Italic = true;
                return 2;
            }
        }

        public class ListToken : MorphTextReport.WikiToken
        {
            // Fields
            private int currentIndentLevel;

            // Methods
            private int getIndentLevel(string s)
            {
                int num = 0;
                while (s[num] == '*')
                {
                    num++;
                }
                return num;
            }

            public override bool match(MorphTextReport.Wiki2WordProcessor p, string s)
            {
                return ((s != null) && s.StartsWith("*"));
            }

            private void OnLineBreak(MorphTextReport.Wiki2WordProcessor p)
            {
                p.Selection.Range.Numbered = false;
                p.LineBreak -= new MorphTextReport.Wiki2WordProcessor.LineBreakEventHandler(this.OnLineBreak);
            }

            public override int process(string s, MorphTextReport.Wiki2WordProcessor p)
            {
                this.currentIndentLevel = this.getIndentLevel(s);
                for (int i = 0; i < this.currentIndentLevel; i++)
                {
                    p.Selection.Range.ListIndent();
                }
                p.LineBreak += new MorphTextReport.Wiki2WordProcessor.LineBreakEventHandler(this.OnLineBreak);
                return this.currentIndentLevel;
            }
        }

        public class NumberedListToken : MorphTextReport.WikiToken
        {
            // Fields
            private int currentIndentLevel;

            // Methods
            private int getIndentLevel(string s)
            {
                int num = 0;
                while (s[num] == '#')
                {
                    num++;
                }
                return num;
            }

            public override bool match(MorphTextReport.Wiki2WordProcessor p, string s)
            {
                return ((s != null) && s.StartsWith("#"));
            }

            private void OnFirstToken(MorphTextReport.Wiki2WordProcessor p, MorphTextReport.WikiToken t)
            {
                p.FirstToken -= new MorphTextReport.Wiki2WordProcessor.FirstTokenInLine(this.OnFirstToken);
            }

            private void OnLineBreak(MorphTextReport.Wiki2WordProcessor p)
            {
                p.Selection.set_Style_Standard();
                p.LineBreak -= new MorphTextReport.Wiki2WordProcessor.LineBreakEventHandler(this.OnLineBreak);
            }

            public override int process(string s, MorphTextReport.Wiki2WordProcessor p)
            {
                this.currentIndentLevel = this.getIndentLevel(s);
                p.Selection.set_Style_Dot_Numbered();
                for (int i = 0; i < (this.currentIndentLevel - 1); i++)
                {
                    p.Selection.Range.ListIndent();
                }
                p.LineBreak += new MorphTextReport.Wiki2WordProcessor.LineBreakEventHandler(this.OnLineBreak);
                p.FirstToken += new MorphTextReport.Wiki2WordProcessor.FirstTokenInLine(this.OnFirstToken);
                return this.currentIndentLevel;
            }
        }

        public class ParagraphFormat
        {
            // Fields
            private iTextSharp.text.Document document;
            private FontProp font = new FontProp();
            private Paragraph paragraph = new Paragraph();
            private ParagraphProp paragraphs = new ParagraphProp();
            private ParagraphType paragraphType;
            public RangeProp range;

            // Methods
            public ParagraphFormat(iTextSharp.text.Document document)
            {
                this.document = document;
                this.range = new RangeProp(this.document);
            }

            public void AddPicture(string path, bool leftAlign)
            {
                Logger.Log("AddPicture: " + path, Logger.ERROR);
                try
                {
                    Image instance = Image.GetInstance(path);
                    this.paragraph.Add(instance);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception, Logger.DEBUG);
                    if (path != null)
                    {
                        FileInfo info = new FileInfo(path);
                        this.paragraph.Add(new Chunk("<Image \"" + info.Name + "\" not found>", MorphToWord.STYLE_NORMAL));
                    }
                    else
                    {
                        this.paragraph.Add(new Chunk("<Image not found>", MorphToWord.STYLE_NORMAL));
                    }
                }
            }

            public void set_Style_Dot_Numbered()
            {
                Logger.Log("set_Style_Dot_Numbered", Logger.ERROR);
                this.paragraphType = ParagraphType.NUMBEREDLIST;
            }

            public void set_Style_Heading(int headingSize)
            {
                Logger.Log("set_Style_Heading: " + headingSize, Logger.ERROR);
                this.paragraphType = ParagraphType.HEADING;
            }

            public void set_Style_Standard()
            {
                Logger.Log("set_Style_Standard", Logger.ERROR);
                this.paragraphType = ParagraphType.STANDARD;
            }

            public void TypeParagraph()
            {
                Logger.Log("TypeParagraph", Logger.ERROR);
                if (this.Range.IndentLevel > 0)
                {
                    this.Range.getCurList().Add(this.paragraph);
                }
                else if ((this.paragraph != null) && (this.document != null))
                {
                    this.document.Add(this.paragraph);
                    this.paragraph.Clear();
                }
                else
                {
                    Logger.Log("paragraph or document are null", Logger.WARNING);
                }
            }

            public void TypeText(string text)
            {
                Logger.Log("type text: " + text, Logger.ERROR);
                int num = 12;
                switch (this.paragraphType)
                {
                    case ParagraphType.HEADING:
                        text = text + "\n";
                        num = 0x12;
                        break;

                    default:
                        num = 12;
                        break;
                }
                int style = 0;
                if (this.Font.Bold)
                {
                    style |= 1;
                    this.Font.Bold = false;
                }
                if (this.Font.Italic)
                {
                    style |= 2;
                    this.Font.Italic = false;
                }
                if (this.Font.Underline)
                {
                    style |= 4;
                    this.Font.Underline = false;
                }
                Chunk o = new Chunk();
                o.Font = FontFactory.GetFont(FontFactory.DefaultEncoding, (float)num, style);
                o.Append(text);
                this.paragraph.Add(o);
            }

            // Properties
            public FontProp Font
            {
                get
                {
                    return this.font;
                }
            }

            public ParagraphProp Paragraphs
            {
                get
                {
                    return this.paragraphs;
                }
            }

            public RangeProp Range
            {
                get
                {
                    return this.range;
                }
            }

            // Nested Types
            public class FontProp
            {
                // Fields
                private bool bold;
                private bool italic;
                private bool underlined;

                // Properties
                public bool Bold
                {
                    get
                    {
                        return this.bold;
                    }
                    set
                    {
                        this.bold = value;
                    }
                }

                public bool Italic
                {
                    get
                    {
                        return this.italic;
                    }
                    set
                    {
                        this.italic = value;
                    }
                }

                public bool Underline
                {
                    get
                    {
                        return this.underlined;
                    }
                    set
                    {
                        this.underlined = value;
                    }
                }
            }

            public class ParagraphProp
            {
                // Fields
                private int indentLevel;

                // Methods
                public void Indent()
                {
                    this.indentLevel++;
                }

                public void Outdent()
                {
                    this.indentLevel--;
                }
            }

            private enum ParagraphType
            {
                STANDARD,
                HEADING,
                NUMBEREDLIST,
                IMAGE
            }

            public class RangeProp
            {
                // Fields
                private iTextSharp.text.Document document;
                private int indentLevel;
                private bool numbered = true;
                private Stack s = new Stack();

                // Methods
                public RangeProp(iTextSharp.text.Document document)
                {
                    this.document = document;
                }

                public void clear()
                {
                    this.s.Clear();
                }

                public List getCurList()
                {
                    if (this.s.Count > 0)
                    {
                        return (List)this.s.Peek();
                    }
                    return new List(this.numbered);
                }

                public void ListIndent()
                {
                    this.indentLevel++;
                    List list = new List(this.numbered);
                    list.Add(this.getCurList());
                    this.s.Push(list);
                }

                public void ListOutdent()
                {
                    this.indentLevel--;
                    List element = (List)this.s.Pop();
                    this.document.Add(element);
                }

                // Properties
                public int IndentLevel
                {
                    get
                    {
                        return this.indentLevel;
                    }
                }

                public bool Numbered
                {
                    set
                    {
                        this.numbered = value;
                    }
                }
            }
        }

        public class PictureToken : MorphTextReport.WikiToken
        {
            // Fields
            private object oFalse = false;
            private object oMissing = Missing.Value;
            private object oTrue = true;

            // Methods
            public void addPicture(MorphTextReport.Wiki2WordProcessor p, string path, bool leftAlign, float scale)
            {
                p.Selection.AddPicture(path, leftAlign);
            }

            public override bool match(MorphTextReport.Wiki2WordProcessor p, string s)
            {
                return ((s != null) && s.StartsWith("[["));
            }

            public override int process(string s, MorphTextReport.Wiki2WordProcessor p)
            {
                string str3;
                int length = s.IndexOf("]]", 0) + 2;
                string[] strArray = s.Substring(0, length).Replace("[[Bild:", "").Replace("[", "").Replace("]", "").Split(new char[] { '|' });
                string title = strArray[0];
                bool flag = false;
                bool leftAlign = false;
                for (int i = 1; i < strArray.Length; i++)
                {
                    string str4 = strArray[i].ToLower();
                    if (str4 == null)
                    {
                        goto Label_00A1;
                    }
                    if (!(str4 == "thumb"))
                    {
                        if (str4 == "left")
                        {
                            goto Label_009C;
                        }
                        goto Label_00A1;
                    }
                    flag = true;
                    continue;
                Label_009C:
                    leftAlign = true;
                    continue;
                Label_00A1:
                    string text1 = strArray[i];
                }
                str3 = str3 = p.getFullPathToImage(title);
                float scale = flag ? 0.5f : 1f;
                this.addPicture(p, str3, leftAlign, scale);
                return length;
            }
        }

        public class TextToken : MorphTextReport.WikiToken
        {
            // Methods
            private int getTextLength(string s)
            {
                int index = 1;
                char[] chArray = s.ToCharArray();
                while ((s.Length > index) && char.IsLetterOrDigit(chArray[index]))
                {
                    index++;
                }
                return index;
            }

            public override bool match(MorphTextReport.Wiki2WordProcessor p, string s)
            {
                return true;
            }

            public override int process(string s, MorphTextReport.Wiki2WordProcessor p)
            {
                int length = this.getTextLength(s);
                p.Selection.TypeText(s.Substring(0, length));
                return length;
            }
        }

        public class UnderlineToken : MorphTextReport.WikiToken
        {
            // Fields
            private bool endToken;

            // Methods
            public override bool match(MorphTextReport.Wiki2WordProcessor p, string s)
            {
                return ((s != null) && s.StartsWith("__"));
            }

            public override int process(string s, MorphTextReport.Wiki2WordProcessor p)
            {
                if (this.endToken)
                {
                    p.Selection.Font.Underline = false;
                }
                else
                {
                    p.Selection.Font.Underline = true;
                }
                this.endToken = !this.endToken;
                return 2;
            }
        }

        public class Wiki2WordProcessor
        {
            // Fields
            private DBData db;
            public static object oEndOfDoc = @"\endofdoc";
            public static object oFalse = false;
            public static object oMissing = Missing.Value;
            public static object oTrue = true;
            private long OwnerUID = -1L;
            private string processedString = "";
            private MorphTextReport.ParagraphFormat selection;
            private ArrayList tokenList = new ArrayList();

            // Events
            public event FirstTokenInLine FirstToken;

            public event LineBreakEventHandler LineBreak;

            // Methods
            public Wiki2WordProcessor(DBData db)
            {
                this.db = db;
                this.tokenList.Add(new MorphTextReport.ItalicBoldToken());
                this.tokenList.Add(new MorphTextReport.BoldToken());
                this.tokenList.Add(new MorphTextReport.ItalicToken());
                this.tokenList.Add(new MorphTextReport.UnderlineToken());
                this.tokenList.Add(new MorphTextReport.IndentToken());
                this.tokenList.Add(new MorphTextReport.PictureToken());
                this.tokenList.Add(new MorphTextReport.HeadingToken());
                this.tokenList.Add(new MorphTextReport.ListToken());
                this.tokenList.Add(new MorphTextReport.NumberedListToken());
                this.tokenList.Add(new MorphTextReport.TextToken());
            }

            public string getFullPathToImage(string title)
            {
                if (this.db != null)
                {
                    return (this.db.WikiImage.ImagesPath + this.db.lookup("FILENAME", "WIKI_IMAGE", string.Concat(new object[] { "TITLE = '", title, "' AND OWNER_UID = ", this.OwnerUID })));
                }
                return null;
            }

            public void run(long OwnerUID, MorphTextReport.ParagraphFormat selection, string s)
            {
                this.OwnerUID = OwnerUID;
                this.selection = selection;
                foreach (string str in s.Replace("\r", "").Split("\n".ToCharArray()))
                {
                    string str2 = str;
                    bool flag = true;
                    while (str2.Length > 0)
                    {
                        int num = 0;
                        while ((num < this.tokenList.Count) && !((MorphTextReport.WikiToken)this.tokenList[num]).match(this, str2))
                        {
                            num++;
                        }
                        MorphTextReport.WikiToken t = (MorphTextReport.WikiToken)this.tokenList[num];
                        if (flag && (this.FirstToken != null))
                        {
                            this.FirstToken(this, t);
                        }
                        flag = false;
                        int length = t.process(str2, this);
                        this.processedString = this.processedString + str2.Substring(0, length);
                        str2 = str2.Substring(length);
                    }
                    selection.TypeParagraph();
                    if (this.LineBreak != null)
                    {
                        this.LineBreak(this);
                    }
                }
            }

            // Properties
            public string ProcessedString
            {
                get
                {
                    return this.processedString;
                }
            }

            public MorphTextReport.ParagraphFormat Selection
            {
                get
                {
                    return this.selection;
                }
            }

            // Nested Types
            public delegate void FirstTokenInLine(MorphTextReport.Wiki2WordProcessor p, MorphTextReport.WikiToken t);

            public delegate void LineBreakEventHandler(MorphTextReport.Wiki2WordProcessor p);
        }

        public abstract class WikiToken
        {
            // Methods
            protected WikiToken()
            {
            }

            public abstract bool match(MorphTextReport.Wiki2WordProcessor p, string s);
            public abstract int process(string s, MorphTextReport.Wiki2WordProcessor p);
        }
    }
}
using ch.appl.psoft.db;
using ch.psoft.Util;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections;
using System.Reflection;

namespace ch.appl.psoft.Morph
{
    public class MorphReport
    {
        public abstract class WikiToken
        {

            public abstract bool match(MorphReport.Wiki2WordProcessor p, string s);

            public abstract int process(string s, MorphReport.Wiki2WordProcessor p);
        }


        public class ItalicBoldToken : MorphReport.WikiToken
        {

            public override bool match(MorphReport.Wiki2WordProcessor p, string s)
            {
                if (s != null && s.StartsWith("\'\'\'\'\'"))
                {
                    return true;
                }
                return false;
            }

            public override int process(string s, MorphReport.Wiki2WordProcessor p)
            {
                p.Selection.Font.Italic = 9999998;
                p.Selection.Font.Bold = 9999998;
                return 5;
            }
        }


        public class BoldToken : MorphReport.WikiToken
        {

            public override bool match(MorphReport.Wiki2WordProcessor p, string s)
            {
                if (s != null && s.StartsWith("\'\'\'"))
                {
                    return true;
                }
                return false;
            }

            public override int process(string s, MorphReport.Wiki2WordProcessor p)
            {
                p.Selection.Font.Bold = 9999998;
                return 3;
            }
        }


        public class ItalicToken : MorphReport.WikiToken
        {

            public override bool match(MorphReport.Wiki2WordProcessor p, string s)
            {
                if (s != null && s.StartsWith("\'\'"))
                {
                    return true;
                }
                return false;
            }

            public override int process(string s, MorphReport.Wiki2WordProcessor p)
            {
                p.Selection.Font.Italic = 9999998;
                return 2;
            }
        }


        public class UnderlineToken : MorphReport.WikiToken
        {
            private bool endToken;


            public override bool match(MorphReport.Wiki2WordProcessor p, string s)
            {
                if (s != null && s.StartsWith("__"))
                {
                    return true;
                }
                return false;
            }

            public override int process(string s, MorphReport.Wiki2WordProcessor p)
            {
                if (endToken)
                {
                    p.Selection.Font.Underline = (WdUnderline)0;
                }
                else
                {
                    p.Selection.Font.Underline = (WdUnderline)1;
                }
                endToken = false;
                return 2;
            }
        }


        public class IndentToken : MorphReport.WikiToken
        {
            private int currentIndentLevel;


            public override bool match(MorphReport.Wiki2WordProcessor p, string s)
            {
                char ch = ' ';
                if (p.ProcessedString.Length > 0)
                {
                    ch = p.ProcessedString[p.ProcessedString.Length - 1];
                }
                if (s != null && Char.IsWhiteSpace(ch) && s.StartsWith(":") && s.TrimEnd(": ".ToCharArray()).Length > 0)
                {
                    return true;
                }
                return false;
            }

            public override int process(string s, MorphReport.Wiki2WordProcessor p)
            {
                int i = getIndentLevel(s);
                if (i > currentIndentLevel)
                {
                    for (int j = currentIndentLevel; j < i; j++)
                    {
                        p.Selection.Paragraphs.Indent();
                    }
                }
                else
                {
                    for (int k = i; k > currentIndentLevel; k--)
                    {
                        p.Selection.Paragraphs.Outdent();
                    }
                }
                return i;
            }

            private int getIndentLevel(string s)
            {
                int i;

                for (i = 1; s[i - 1] == ':'; i++)
                {
                }
                return i;
            }
        }


        public class TextToken : MorphReport.WikiToken
        {

            public override bool match(MorphReport.Wiki2WordProcessor p, string s)
            {
                return true;
            }

            public override int process(string s, MorphReport.Wiki2WordProcessor p)
            {
                int i = getTextLength(s);
                p.Selection.TypeText(s.Substring(0, i));
                return i;
            }

            private int getTextLength(string s)
            {
                int i = 1;
                for (char[] chs = s.ToCharArray(); s.Length > i && Char.IsLetterOrDigit(chs[i]); i++)
                {
                }
                return i;
            }
        }


        public class HeadingToken : MorphReport.WikiToken
        {

            public override bool match(MorphReport.Wiki2WordProcessor p, string s)
            {
                if (s != null && s.StartsWith("="))
                {
                    return true;
                }
                return false;
            }

            public override int process(string s, MorphReport.Wiki2WordProcessor p)
            {
                int i = getHeadingSize(s);
                object obj = String.Concat("\u00DCberschrift \uFFFD", i);
                p.Selection.set_Style(ref obj);
                p.LineBreak += new MorphReport.Wiki2WordProcessor.LineBreakEventHandler(OnLineBreak);
                return i;
            }


            private int getHeadingSize(string s)
            {
                int i;

                for (i = 1; s[i - 1] == '='; i++)
                {
                }
                return i - 1;
            }

            private void OnLineBreak(MorphReport.Wiki2WordProcessor p)
            {
                object local = "Standard";
                p.Selection.set_Style(ref local);
                p.LineBreak -= new MorphReport.Wiki2WordProcessor.LineBreakEventHandler(this.OnLineBreak);
            }
        }


        public class PictureToken : MorphReport.WikiToken
        {
            private object oFalse = 0;

            private object oTrue = 1;

            private object oMissing = Missing.Value;


            public override bool match(MorphReport.Wiki2WordProcessor p, string s)
            {
                if (s != null && s.StartsWith("[["))
                {
                    return true;
                }
                return false;
            }

            public override int process(string s, MorphReport.Wiki2WordProcessor p)
            {
               p.Selection.Font.Italic = 0x98967e;
               p.Selection.Font.Bold = 0x98967e;
               return 5;
            }

            public void addPicture(MorphReport.Wiki2WordProcessor p, string path, bool leftAlign, float scale)
            {
                Shape shape = p.Selection.InlineShapes.AddPicture(path, ref oMissing, ref oTrue, ref oMissing).ConvertToShape();
                shape.WrapFormat.Type = (WdWrapType)0;
                if (leftAlign)
                {
                    shape.Left = -999998.0F;
                }
                else
                {
                    shape.Left = -999996.0F;
                }
                shape.Top = -999999.0F;
                shape.RelativeVerticalPosition = (WdRelativeVerticalPosition)3;
                shape.RelativeHorizontalPosition = (WdRelativeHorizontalPosition)0;
            }
        }


        public class ListToken : MorphReport.WikiToken
        {
            private int currentIndentLevel;


            public override bool match(MorphReport.Wiki2WordProcessor p, string s)
            {
                if (s != null && s.StartsWith("*"))
                {
                    return true;
                }
                return false;
            }

            public override int process(string s, MorphReport.Wiki2WordProcessor p)
            {
                this.currentIndentLevel = this.getIndentLevel(s);
                object obj2 = 1;
                object wdListApplyToSelection = WdListApplyTo.wdListApplyToSelection;
                object defaultListBehavior = WdDefaultListBehavior.wdWord10ListBehavior;
                p.Selection.Range.ListFormat.ApplyListTemplate(p.Selection.Application.ListGalleries[WdListGalleryType.wdNumberGallery].ListTemplates.get_Item(ref obj2), ref MorphReport.Wiki2WordProcessor.oFalse, ref wdListApplyToSelection, ref defaultListBehavior);
                for (int i = 0; i < this.currentIndentLevel; i++)
                {
                    p.Selection.Range.ListFormat.ListIndent();
                }
                p.LineBreak += new MorphReport.Wiki2WordProcessor.LineBreakEventHandler(this.OnLineBreak);
                return this.currentIndentLevel;
            }

            private int getIndentLevel(string s)
            {
                int i;

                for (i = 0; s[i] == '*'; i++)
                {
                }
                return i;
            }

            private void OnLineBreak(MorphReport.Wiki2WordProcessor p)
            {
                object local = 1;
                p.Selection.Range.ListFormat.RemoveNumbers(ref local);
                p.LineBreak -= new MorphReport.Wiki2WordProcessor.LineBreakEventHandler(this.OnLineBreak);
            }
        }


        public class NumberedListToken : MorphReport.WikiToken
        {
            private int currentIndentLevel;


            public override bool match(MorphReport.Wiki2WordProcessor p, string s)
            {
                if (s != null && s.StartsWith("#"))
                {
                    return true;
                }
                return false;
            }

            public override int process(string s, MorphReport.Wiki2WordProcessor p)
            {
                currentIndentLevel = getIndentLevel(s);
                object local = "1 / 1.1 / 1.1.1";
                p.Selection.set_Style(ref local);
                for (int i = 0; i < currentIndentLevel - 1; i++)
                {
                    p.Selection.Range.ListFormat.ListIndent();
                }
                p.LineBreak += new MorphReport.Wiki2WordProcessor.LineBreakEventHandler(this.OnLineBreak);
                p.FirstToken += new MorphReport.Wiki2WordProcessor.FirstTokenInLine(this.OnFirstToken);
                return currentIndentLevel;
            }

            private int getIndentLevel(string s)
            {
                int i;

                for (i = 0; s[i] == '#'; i++)
                {
                }
                return i;
            }

            private void OnLineBreak(MorphReport.Wiki2WordProcessor p)
            {
                object local = "Standard";
                p.Selection.set_Style(ref local);
                p.LineBreak -= new MorphReport.Wiki2WordProcessor.LineBreakEventHandler(this.OnLineBreak);
            }

            private void OnFirstToken(MorphReport.Wiki2WordProcessor p, MorphReport.WikiToken t)
            {
                if (t != this)
                {
                    object obj = 1;
                    p.Selection.Application.ListGalleries[WdListGalleryType.wdNumberGallery].ListTemplates.get_Item(ref obj).ListLevels[1].StartAt = 1;
                }
                p.FirstToken -= new MorphReport.Wiki2WordProcessor.FirstTokenInLine(OnFirstToken);
            }
        }


        public class Wiki2WordProcessor
        {
            public delegate void LineBreakEventHandler(MorphReport.Wiki2WordProcessor p);

            public delegate void FirstTokenInLine(MorphReport.Wiki2WordProcessor p, MorphReport.WikiToken t);

            public static object oFalse = 0;

            public static object oTrue = 1;

            public static object oMissing = Missing.Value;

            public static object oEndOfDoc = "\\endofdoc";

            private Selection selection;

            private string processedString = "";

            private ArrayList tokenList = new ArrayList();

            private long OwnerUID = -1;

            private DBData db;


            public Selection Selection
            {
                get
                {
                    return selection;
                }
            }

            public string ProcessedString
            {
                get
                {
                    return processedString;
                }
            }

            public event LineBreakEventHandler LineBreak;

            public event FirstTokenInLine FirstToken;

            public Wiki2WordProcessor(DBData db)
            {
                this.db = db;
                tokenList.Add(new MorphReport.ItalicBoldToken());
                tokenList.Add(new MorphReport.BoldToken());
                tokenList.Add(new MorphReport.ItalicToken());
                tokenList.Add(new MorphReport.UnderlineToken());
                tokenList.Add(new MorphReport.IndentToken());
                tokenList.Add(new MorphReport.PictureToken());
                tokenList.Add(new MorphReport.HeadingToken());
                tokenList.Add(new MorphReport.ListToken());
                tokenList.Add(new MorphReport.NumberedListToken());
                tokenList.Add(new MorphReport.TextToken());
            }

            public void run(long OwnerUID, Selection selection, string s)
            {
                this.OwnerUID = OwnerUID;
                this.selection = selection;
                string[] strs2 = s.Replace("\r", "").Split("\n".ToCharArray());
                for (int k = 0; k < (int)strs2.Length; k++)
                {
                    string str2 = strs2[k];
                    bool flag = true;
                    int j;

                    for (; str2.Length > 0; str2 = str2.Substring(j))
                    {
                        int i;

                        for (i = 0; i < tokenList.Count && !((MorphReport.WikiToken)tokenList[i]).match(this, str2); i++)
                        {
                        }
                        MorphReport.WikiToken morphReport_WikiToken = (MorphReport.WikiToken)tokenList[i];
                        if (flag && FirstToken != null)
                        {
                            FirstToken(this, morphReport_WikiToken);
                        }
                        flag = false;
                        j = morphReport_WikiToken.process(str2, this);
                        processedString = String.Concat(processedString, str2.Substring(0, j));
                    }
                    selection.TypeParagraph();
                    if (LineBreak != null)
                    {
                        LineBreak(this);
                    }
                }
            }

            public string getFullPathToImage(string title)
            {
                return String.Concat(db.WikiImage.ImagesPath, db.lookup("FILENAME", "WIKI_IMAGE", String.Concat(new object[] { "TITLE = \'", title, "\' AND OWNER_UID = ", OwnerUID })));
            }
        }


        public static object oFalse = 0;

        public static object oTrue = 1;

        public static object oMissing = Missing.Value;

        public static object oEndOfDoc = "\\endofdoc";


        public void create(DBData db, long OwnerUID, string fileName, string text)
        {
            try
            {
                String.Concat(Global.Config.documentSaveDirectory, "\\", fileName);
                string str = fileName.Replace(".htm", ".doc");
                object local1 = String.Concat(Global.Config.documentSaveDirectory, "\\x\\", str);
                _Application _Application = new ApplicationClass();
                _Application.Visible = true;
                _Document _Document = _Application.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                Selection selection = _Application.Selection;
                new Wiki2WordProcessor(db).run(OwnerUID, selection, text);
                object local2 = 0;
                _Document.SaveAs(ref local1, ref local2, ref oMissing, ref oMissing, ref oFalse, ref oMissing, ref oMissing, ref oMissing, ref oTrue, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                _Document.Saved = true;
            }
            catch (Exception e)
            {
                Logger.Log(e, Logger.ERROR);
            }
        }
    }
}

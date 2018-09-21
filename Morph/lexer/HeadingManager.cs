using iTextSharp.text;
using System.Text;
namespace ch.appl.psoft.Morph.lexer
{
    public class HeadingManager
    {
        // Fields
        private Font font = MorphToWord.STYLE_NORMAL;
        private int headingLen = 1;
        private Phrase phrase = new Phrase();
        private Font prev = MorphToWord.STYLE_NORMAL;
        private bool stateBold;
        private StringBuilder stream = new StringBuilder();

        // Methods
        public HeadingManager(string yytext)
        {
            this.headingLen = yytext.LastIndexOf("=") + 1;
        }

        public void append(string s)
        {
            this.stream.Append(s);
        }

        internal void flush()
        {
            this.phrase.Add(new Chunk(this.stream.ToString(), this.font));
        }

        public Paragraph formatParagraph(int startHeading)
        {
            int num = (this.headingLen + startHeading) - 1;
            switch (num)
            {
                case 0:
                case 1:
                    return new Paragraph(this.phrase.Content, MorphToWord.STYLE_HEADING_1);

                case 2:
                    return new Paragraph(this.phrase.Content, MorphToWord.STYLE_HEADING_2);

                case 3:
                    return new Paragraph(this.phrase.Content, MorphToWord.STYLE_HEADING_3);
            }
            if (num >= MorphToWord.MAXHEADING)
            {
                return new Paragraph(this.phrase.Content, MorphToWord.STYLE_NORMAL);
            }
            return new Paragraph(this.phrase.Content, MorphToWord.STYLE_HEADING_ARRAY[num - 4]);
        }

        internal void setBold()
        {
            this.stateBold = this.setFont(this.stateBold, 1);
        }

        internal void setBoldItalic()
        {
            this.stateBold = this.setFont(this.stateBold, 3);
        }

        internal bool setFont(bool isset, int type)
        {
            this.phrase.Add(new Chunk(this.stream.ToString(), this.font));
            this.stream = new StringBuilder();
            if (isset)
            {
                this.font = this.prev;
                return false;
            }
            this.prev = this.font;
            Font font = new Font(this.font);
            font.SetStyle(type);
            this.font = font;
            return true;
        }

        internal void setItalic()
        {
            this.stateBold = this.setFont(this.stateBold, 2);
        }

        internal void setUnderline()
        {
            this.stateBold = this.setFont(this.stateBold, 4);
        }
    }
}
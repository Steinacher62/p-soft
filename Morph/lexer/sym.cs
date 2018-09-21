namespace ch.appl.psoft.Morph.lexer
{
    internal class sym
    {
        // Fields
        public const int ANY = 9;
        public const int BOLD = 5;
        public const int BOLDITALIC = 7;
        public const int CHAR = 10;
        public const int EMAIL = 15;
        public const int EOL = 8;
        public const int HEADING = 12;
        public const int HLINE = 0x10;
        public const int IMAGE = 1;
        public const int IMAGE_THUMB = 0;
        public const int INTERNAL_LINK = 20;
        public const int ITALIC = 6;
        public const int LISTNUMB = 2;
        public const int LISTSIMPLE = 4;
        public const int LISTUNNUMB = 3;
        public const int PARAGRAPH_BLOCK = 0x13;
        public const int PARAGRAPH_CENTER = 0x12;
        public const int PARAGRAPH_RIGHT = 0x11;
        public const int SIMPLEURL = 13;
        public const int UNDEFINED = 0x15;
        public const int UNDERLINED = 11;
        public const int URL = 14;

        // Nested Types
        private enum terminals
        {
            IMAGE_THUMB,
            IMAGE,
            LISTNUMB,
            LISTUNNUMB,
            LISTSIMPLE,
            BOLD,
            ITALIC,
            BOLDITALIC,
            EOL,
            ANY,
            CHAR,
            UNDERLINED,
            HEADING,
            SIMPLEURL,
            URL,
            EMAIL,
            HLINE,
            PARAGRAPH_RIGHT,
            PARAGRAPH_CENTER,
            PARAGRAPH_BLOCK,
            INTERNAL_LINK,
            UNDEFINED
        }
    }
}

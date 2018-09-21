using ch.appl.psoft.Morph.lexer;
using System;
using System.IO;
namespace ch.appl.psoft.Morph
{
    internal class Lexer
    {
        // Fields
        private HeadingManager headingManager;
        private int yychar;
        private int yycolumn;
        public const int YYEOF = -1;
        public const int YYHEADING = 2;
        public const int YYINITIAL = 0;
        private int yyline;
        private static readonly int[] ZZ_ACTION = zzUnpackAction();
        private static readonly ushort[] ZZ_ACTION_PACKED_0 = new ushort[] { 
        3, 0, 1, 1, 2, 2, 10, 1, 1, 3, 1, 4, 1, 5, 1, 6, 
        1, 7, 2, 8, 2, 7, 3, 0, 1, 9, 1, 0, 1, 10, 3, 0, 
        2, 11, 1, 12, 1, 13, 1, 14, 1, 0, 1, 6, 1, 15, 1, 0x10, 
        3, 0, 1, 0x11, 5, 0, 1, 6, 1, 0x12, 14, 0, 1, 0x13, 4, 0, 
        2, 20, 1, 0x15, 2, 0, 1, 0x16, 7, 0, 1, 0x16, 5, 0, 2, 0x17, 
        7, 0, 1, 0x17, 6, 0, 1, 0x18, 8, 0, 1, 0x19, 7, 0, 1, 0x1a, 
        2, 0, 1, 0x1b, 10, 0, 0
     };
        private static readonly int[] ZZ_ATTRIBUTE = zzUnpackAttribute();
        private static readonly ushort[] ZZ_ATTRIBUTE_PACKED_0 = new ushort[] { 
        3, 0, 1, 9, 1, 1, 1, 9, 14, 1, 1, 9, 1, 1, 1, 9, 
        2, 1, 3, 0, 1, 1, 1, 0, 1, 1, 3, 0, 1, 1, 4, 9, 
        1, 0, 1, 1, 1, 9, 1, 1, 3, 0, 1, 1, 5, 0, 1, 9, 
        1, 1, 14, 0, 1, 9, 4, 0, 1, 1, 2, 9, 2, 0, 1, 1, 
        7, 0, 1, 1, 5, 0, 2, 1, 7, 0, 1, 1, 6, 0, 1, 9, 
        8, 0, 1, 9, 7, 0, 1, 9, 2, 0, 1, 9, 10, 0, 0
     };
        private const int ZZ_BUFFERSIZE = 0x4000;
        private static readonly char[] ZZ_CMAP = zzUnpackCMap(ZZ_CMAP_PACKED);
        private static readonly ushort[] ZZ_CMAP_PACKED = new ushort[] { 
        9, 0, 1, 30, 1, 2, 2, 0, 1, 1, 0x12, 0, 1, 0x1c, 2, 0, 
        1, 13, 3, 0, 1, 15, 2, 0, 1, 14, 2, 0, 1, 5, 1, 4, 
        1, 7, 10, 6, 1, 12, 1, 0, 1, 0x22, 1, 0x1b, 1, 0x21, 1, 0, 
        1, 0x20, 1, 3, 1, 0x11, 1, 3, 1, 0x18, 4, 3, 1, 0x17, 11, 3, 
        1, 0x16, 5, 3, 1, 0x10, 1, 0x1d, 1, 0x15, 1, 0, 1, 8, 1, 0, 
        3, 3, 1, 20, 3, 3, 1, 9, 1, 0x12, 2, 3, 1, 0x13, 3, 3, 
        1, 11, 3, 3, 1, 10, 6, 3, 1, 0, 1, 0x1a, 0x2d, 0, 1, 0x1f, 
        10, 0, 1, 0x1f, 4, 0, 1, 0x1f, 5, 0, 0x17, 0x1f, 1, 0, 0x1f, 0x1f, 
        1, 0, 0x128, 0x1f, 2, 0, 0x12, 0x1f, 0x1c, 0, 0x5e, 0x1f, 2, 0, 9, 0x1f, 
        2, 0, 7, 0x1f, 14, 0, 2, 0x1f, 14, 0, 5, 0x1f, 9, 0, 1, 0x1f, 
        0x8b, 0, 1, 0x1f, 11, 0, 1, 0x1f, 1, 0, 3, 0x1f, 1, 0, 1, 0x1f, 
        1, 0, 20, 0x1f, 1, 0, 0x2c, 0x1f, 1, 0, 8, 0x1f, 2, 0, 0x1a, 0x1f, 
        12, 0, 130, 0x1f, 10, 0, 0x39, 0x1f, 2, 0, 2, 0x1f, 2, 0, 2, 0x1f, 
        3, 0, 0x26, 0x1f, 2, 0, 2, 0x1f, 0x37, 0, 0x26, 0x1f, 2, 0, 1, 0x1f, 
        7, 0, 0x27, 0x1f, 0x48, 0, 0x1b, 0x1f, 5, 0, 3, 0x1f, 0x2e, 0, 0x1a, 0x1f, 
        5, 0, 11, 0x1f, 0x15, 0, 10, 0x19, 7, 0, 0x63, 0x1f, 1, 0, 1, 0x1f, 
        15, 0, 2, 0x1f, 9, 0, 10, 0x19, 3, 0x1f, 0x13, 0, 1, 0x1f, 1, 0, 
        0x1b, 0x1f, 0x53, 0, 0x26, 0x1f, 0x15f, 0, 0x35, 0x1f, 3, 0, 1, 0x1f, 0x12, 0, 
        1, 0x1f, 7, 0, 10, 0x1f, 4, 0, 10, 0x19, 0x15, 0, 8, 0x1f, 2, 0, 
        2, 0x1f, 2, 0, 0x16, 0x1f, 1, 0, 7, 0x1f, 1, 0, 1, 0x1f, 3, 0, 
        4, 0x1f, 0x22, 0, 2, 0x1f, 1, 0, 3, 0x1f, 4, 0, 10, 0x19, 2, 0x1f, 
        0x13, 0, 6, 0x1f, 4, 0, 2, 0x1f, 2, 0, 0x16, 0x1f, 1, 0, 7, 0x1f, 
        1, 0, 2, 0x1f, 1, 0, 2, 0x1f, 1, 0, 2, 0x1f, 0x1f, 0, 4, 0x1f, 
        1, 0, 1, 0x1f, 7, 0, 10, 0x19, 2, 0, 3, 0x1f, 0x10, 0, 7, 0x1f, 
        1, 0, 1, 0x1f, 1, 0, 3, 0x1f, 1, 0, 0x16, 0x1f, 1, 0, 7, 0x1f, 
        1, 0, 2, 0x1f, 1, 0, 5, 0x1f, 3, 0, 1, 0x1f, 0x12, 0, 1, 0x1f, 
        15, 0, 1, 0x1f, 5, 0, 10, 0x19, 0x15, 0, 8, 0x1f, 2, 0, 2, 0x1f, 
        2, 0, 0x16, 0x1f, 1, 0, 7, 0x1f, 1, 0, 2, 0x1f, 2, 0, 4, 0x1f, 
        3, 0, 1, 0x1f, 30, 0, 2, 0x1f, 1, 0, 3, 0x1f, 4, 0, 10, 0x19, 
        0x15, 0, 6, 0x1f, 3, 0, 3, 0x1f, 1, 0, 4, 0x1f, 3, 0, 2, 0x1f, 
        1, 0, 1, 0x1f, 1, 0, 2, 0x1f, 3, 0, 2, 0x1f, 3, 0, 3, 0x1f, 
        3, 0, 8, 0x1f, 1, 0, 3, 0x1f, 0x2d, 0, 9, 0x19, 0x15, 0, 8, 0x1f, 
        1, 0, 3, 0x1f, 1, 0, 0x17, 0x1f, 1, 0, 10, 0x1f, 1, 0, 5, 0x1f, 
        0x26, 0, 2, 0x1f, 4, 0, 10, 0x19, 0x15, 0, 8, 0x1f, 1, 0, 3, 0x1f, 
        1, 0, 0x17, 0x1f, 1, 0, 10, 0x1f, 1, 0, 5, 0x1f, 0x24, 0, 1, 0x1f, 
        1, 0, 2, 0x1f, 4, 0, 10, 0x19, 0x15, 0, 8, 0x1f, 1, 0, 3, 0x1f, 
        1, 0, 0x17, 0x1f, 1, 0, 0x10, 0x1f, 0x26, 0, 2, 0x1f, 4, 0, 10, 0x19, 
        0x15, 0, 0x12, 0x1f, 3, 0, 0x18, 0x1f, 1, 0, 9, 0x1f, 1, 0, 1, 0x1f, 
        2, 0, 7, 0x1f, 0x3a, 0, 0x30, 0x1f, 1, 0, 2, 0x1f, 12, 0, 7, 0x1f, 
        9, 0, 10, 0x19, 0x27, 0, 2, 0x1f, 1, 0, 1, 0x1f, 2, 0, 2, 0x1f, 
        1, 0, 1, 0x1f, 2, 0, 1, 0x1f, 6, 0, 4, 0x1f, 1, 0, 7, 0x1f, 
        1, 0, 3, 0x1f, 1, 0, 1, 0x1f, 1, 0, 1, 0x1f, 2, 0, 2, 0x1f, 
        1, 0, 4, 0x1f, 1, 0, 2, 0x1f, 9, 0, 1, 0x1f, 2, 0, 5, 0x1f, 
        1, 0, 1, 0x1f, 9, 0, 10, 0x19, 2, 0, 2, 0x1f, 0x22, 0, 1, 0x1f, 
        0x1f, 0, 10, 0x19, 0x16, 0, 8, 0x1f, 1, 0, 0x22, 0x1f, 0x1d, 0, 4, 0x1f, 
        0x74, 0, 0x22, 0x1f, 1, 0, 5, 0x1f, 1, 0, 2, 0x1f, 0x15, 0, 10, 0x19, 
        6, 0, 6, 0x1f, 0x4a, 0, 0x26, 0x1f, 10, 0, 0x27, 0x1f, 9, 0, 90, 0x1f, 
        5, 0, 0x44, 0x1f, 5, 0, 0x52, 0x1f, 6, 0, 7, 0x1f, 1, 0, 0x3f, 0x1f, 
        1, 0, 1, 0x1f, 1, 0, 4, 0x1f, 2, 0, 7, 0x1f, 1, 0, 1, 0x1f, 
        1, 0, 4, 0x1f, 2, 0, 0x27, 0x1f, 1, 0, 1, 0x1f, 1, 0, 4, 0x1f, 
        2, 0, 0x1f, 0x1f, 1, 0, 1, 0x1f, 1, 0, 4, 0x1f, 2, 0, 7, 0x1f, 
        1, 0, 1, 0x1f, 1, 0, 4, 0x1f, 2, 0, 7, 0x1f, 1, 0, 7, 0x1f, 
        1, 0, 0x17, 0x1f, 1, 0, 0x1f, 0x1f, 1, 0, 1, 0x1f, 1, 0, 4, 0x1f, 
        2, 0, 7, 0x1f, 1, 0, 0x27, 0x1f, 1, 0, 0x13, 0x1f, 14, 0, 9, 0x19, 
        0x2e, 0, 0x55, 0x1f, 12, 0, 620, 0x1f, 2, 0, 8, 0x1f, 10, 0, 0x1a, 0x1f, 
        5, 0, 0x4b, 0x1f, 0x95, 0, 0x34, 0x1f, 0x2c, 0, 10, 0x19, 0x26, 0, 10, 0x19, 
        6, 0, 0x58, 0x1f, 8, 0, 0x29, 0x1f, 0x557, 0, 0x9c, 0x1f, 4, 0, 90, 0x1f, 
        6, 0, 0x16, 0x1f, 2, 0, 6, 0x1f, 2, 0, 0x26, 0x1f, 2, 0, 6, 0x1f, 
        2, 0, 8, 0x1f, 1, 0, 1, 0x1f, 1, 0, 1, 0x1f, 1, 0, 1, 0x1f, 
        1, 0, 0x1f, 0x1f, 2, 0, 0x35, 0x1f, 1, 0, 7, 0x1f, 1, 0, 1, 0x1f, 
        3, 0, 3, 0x1f, 1, 0, 7, 0x1f, 3, 0, 4, 0x1f, 2, 0, 6, 0x1f, 
        4, 0, 13, 0x1f, 5, 0, 3, 0x1f, 1, 0, 7, 0x1f, 130, 0, 1, 0x1f, 
        130, 0, 1, 0x1f, 4, 0, 1, 0x1f, 2, 0, 10, 0x1f, 1, 0, 1, 0x1f, 
        3, 0, 5, 0x1f, 6, 0, 1, 0x1f, 1, 0, 1, 0x1f, 1, 0, 1, 0x1f, 
        1, 0, 4, 0x1f, 1, 0, 3, 0x1f, 1, 0, 7, 0x1f, 0xecb, 0, 2, 0x1f, 
        0x2a, 0, 5, 0x1f, 11, 0, 0x54, 0x1f, 8, 0, 2, 0x1f, 2, 0, 90, 0x1f, 
        1, 0, 3, 0x1f, 6, 0, 40, 0x1f, 4, 0, 0x5e, 0x1f, 0x11, 0, 0x18, 0x1f, 
        0x248, 0, 0x19b6, 0x1f, 0x4a, 0, 0x51a6, 0x1f, 90, 0, 0x48d, 0x1f, 0x773, 0, 0x2ba4, 0x1f, 
        0x215c, 0, 0x12e, 0x1f, 210, 0, 7, 0x1f, 12, 0, 5, 0x1f, 5, 0, 1, 0x1f, 
        1, 0, 10, 0x1f, 1, 0, 13, 0x1f, 1, 0, 5, 0x1f, 1, 0, 1, 0x1f, 
        1, 0, 2, 0x1f, 1, 0, 2, 0x1f, 1, 0, 0x6c, 0x1f, 0x21, 0, 0x16b, 0x1f, 
        0x12, 0, 0x40, 0x1f, 2, 0, 0x36, 0x1f, 40, 0, 12, 0x1f, 0x74, 0, 3, 0x1f, 
        1, 0, 1, 0x1f, 1, 0, 0x87, 0x1f, 0x13, 0, 10, 0x19, 7, 0, 0x1a, 0x1f, 
        6, 0, 0x1a, 0x1f, 11, 0, 0x59, 0x1f, 3, 0, 6, 0x1f, 2, 0, 6, 0x1f, 
        2, 0, 6, 0x1f, 2, 0, 3, 0x1f, 0x23, 0, 0
     };
        private static readonly string[] ZZ_ERROR_MSG = new string[] { "Unkown internal scanner error", "Error: could not match input", "Error: pushback value was too large" };
        private static readonly int[] ZZ_LEXSTATE = new int[] { 0, 1, 2, 2 };
        private const int ZZ_NO_MATCH = 1;
        private const int ZZ_PUSHBACK_2BIG = 2;
        private static readonly int[] ZZ_ROWMAP = zzUnpackRowMap();
        private static readonly ushort[] ZZ_ROWMAP_PACKED_0 = new ushort[] { 
        0, 0, 0, 0x23, 0, 70, 0, 0x69, 0, 140, 0, 0x69, 0, 0xaf, 0, 210, 
        0, 0xf5, 0, 280, 0, 0x13b, 0, 350, 0, 0x181, 0, 420, 0, 0x1c7, 0, 490, 
        0, 0x20d, 0, 560, 0, 0x253, 0, 630, 0, 0x69, 0, 0x299, 0, 0x69, 0, 700, 
        0, 0x2df, 0, 770, 0, 210, 0, 0x325, 0, 210, 0, 840, 0, 0x36b, 0, 910, 
        0, 0x3b1, 0, 980, 0, 0x3f7, 0, 0x69, 0, 0x69, 0, 0x69, 0, 0x69, 0, 0x41a, 
        0, 0x43d, 0, 0x69, 0, 0x460, 0, 0x483, 0, 0x4a6, 0, 0x4c9, 0, 0x4ec, 0, 0x50f, 
        0, 0x532, 0, 0x555, 0, 0x578, 0, 0x59b, 0, 0x69, 0, 0x5be, 0, 0x5e1, 0, 0x604, 
        0, 0x627, 0, 0x64a, 0, 0x66d, 0, 0x690, 0, 0x6b3, 0, 0x6d6, 0, 0x6f9, 0, 0x71c, 
        0, 0x73f, 0, 0x762, 0, 0x785, 0, 0x7a8, 0, 0x69, 0, 0x7cb, 0, 0x7ee, 0, 0x811, 
        0, 0x834, 0, 0x857, 0, 0x69, 0, 0x69, 0, 0x87a, 0, 0x89d, 0, 0x8c0, 0, 0x8e3, 
        0, 0x906, 0, 0x929, 0, 0x94c, 0, 0x96f, 0, 0x992, 0, 0x9b5, 0, 0x4a6, 0, 0x9d8, 
        0, 0x9fb, 0, 0xa1e, 0, 0xa41, 0, 0xa64, 0, 0xa87, 0, 0xaaa, 0, 0xacd, 0, 0xaf0, 
        0, 0xb13, 0, 0xb36, 0, 0xb59, 0, 0xb7c, 0, 0xb9f, 0, 0xbc2, 0, 0xbe5, 0, 0xc08, 
        0, 0xc2b, 0, 0xc4e, 0, 0xc71, 0, 0xc94, 0, 0x69, 0, 0xcb7, 0, 0xcda, 0, 0xcfd, 
        0, 0xd20, 0, 0xd43, 0, 0xd66, 0, 0xd89, 0, 0xdac, 0, 0x69, 0, 0xdcf, 0, 0xdf2, 
        0, 0xe15, 0, 0xe38, 0, 0xe5b, 0, 0xe7e, 0, 0xea1, 0, 0x69, 0, 0xec4, 0, 0xee7, 
        0, 0x69, 0, 0xf0a, 0, 0xf2d, 0, 0xf50, 0, 0xf73, 0, 0xf96, 0, 0xfb9, 0, 0xfdc, 
        0, 0xfff, 0, 0x1022, 0, 0x1045, 0
     };
        public static readonly bool ZZ_SPURIOUS_WARNINGS_SUCK = true;
        private static readonly int[] ZZ_TRANS = zzUnpackTrans();
        private static readonly ushort[] ZZ_TRANS_PACKED_0 = new ushort[] { 
        1, 4, 1, 5, 1, 6, 1, 7, 1, 8, 1, 4, 1, 8, 1, 4, 
        1, 9, 1, 10, 2, 7, 3, 4, 1, 11, 1, 12, 4, 7, 1, 4, 
        3, 7, 1, 8, 3, 4, 1, 13, 1, 4, 1, 8, 1, 4, 1, 14, 
        1, 15, 1, 4, 1, 5, 1, 6, 1, 7, 1, 8, 1, 0x10, 1, 8, 
        1, 4, 1, 9, 1, 10, 2, 7, 1, 0x11, 1, 0x12, 1, 0x13, 1, 11, 
        1, 12, 4, 7, 1, 4, 3, 7, 1, 8, 1, 4, 1, 20, 1, 4, 
        1, 13, 1, 4, 1, 8, 1, 4, 1, 14, 1, 15, 1, 0x15, 1, 0x16, 
        1, 0x17, 5, 0x15, 1, 0x18, 6, 0x15, 1, 0x19, 0x13, 0x15, 0x25, 0, 1, 6, 
        0x23, 0, 1, 0x1a, 1, 0x1b, 1, 0, 1, 0x1b, 1, 0, 1, 0x1b, 3, 0x1a, 
        5, 0, 4, 0x1a, 1, 0, 3, 0x1a, 1, 0x1b, 5, 0, 1, 0x1b, 1, 0x1c, 
        5, 0, 2, 0x1b, 1, 0, 1, 0x1b, 1, 0, 4, 0x1b, 5, 0, 4, 0x1b, 
        1, 0, 4, 0x1b, 5, 0, 1, 0x1b, 1, 0x1c, 5, 0, 2, 0x1b, 1, 0, 
        1, 0x1b, 1, 0, 1, 0x1d, 3, 0x1b, 5, 0, 4, 0x1b, 1, 0, 4, 0x1b, 
        5, 0, 1, 0x1b, 1, 0x1c, 5, 0, 1, 0x1a, 1, 0x1b, 1, 0, 1, 0x1b, 
        1, 0, 1, 0x1b, 1, 0x1a, 1, 30, 1, 0x1a, 5, 0, 4, 0x1a, 1, 0, 
        3, 0x1a, 1, 0x1b, 5, 0, 1, 0x1b, 1, 0x1c, 0x11, 0, 1, 0x1f, 0x16, 0, 
        1, 0x20, 5, 0, 1, 0x21, 2, 0x20, 4, 0, 1, 0x22, 4, 0x20, 1, 0, 
        3, 0x20, 11, 0, 1, 0x23, 1, 0x24, 0x41, 0, 1, 0x25, 1, 0x26, 0x21, 0, 
        1, 0x27, 6, 0, 1, 40, 0x29, 0, 1, 0x11, 0x23, 0, 1, 0x12, 0x23, 0, 
        1, 0x13, 0x2f, 0, 1, 20, 1, 0x29, 8, 0, 1, 0x17, 40, 0, 1, 0x2a, 
        0x29, 0, 1, 0x2b, 0x16, 0, 1, 0x2c, 1, 0x1b, 1, 0, 1, 0x1b, 1, 0, 
        1, 0x1b, 3, 0x2c, 5, 0, 4, 0x2c, 1, 0, 3, 0x2c, 1, 0x1b, 5, 0, 
        1, 0x1b, 1, 0x1c, 5, 0, 2, 0x2d, 1, 0, 1, 0x2d, 1, 0, 4, 0x2d, 
        5, 0, 4, 0x2d, 1, 0, 4, 0x2d, 5, 0, 1, 0x2d, 6, 0, 1, 0x2c, 
        1, 0x1b, 1, 0, 1, 0x1b, 1, 0, 1, 0x1b, 1, 0x2c, 1, 0x2e, 1, 0x2c, 
        5, 0, 4, 0x2c, 1, 0, 3, 0x2c, 1, 0x1b, 5, 0, 1, 0x1b, 1, 0x1c, 
        0x11, 0, 1, 0x2f, 0x16, 0, 1, 0x30, 5, 0, 3, 0x30, 5, 0, 4, 0x30, 
        1, 0, 3, 0x30, 13, 0, 1, 0x30, 5, 0, 1, 0x30, 1, 0x31, 1, 0x30, 
        5, 0, 4, 0x30, 1, 0, 3, 0x30, 0x1b, 0, 1, 50, 4, 0, 1, 0x33, 
        14, 0, 1, 0x24, 0x25, 0, 1, 0x34, 0x2a, 0, 1, 0x35, 0x24, 0, 1, 0x36, 
        0x16, 0, 1, 0x1b, 1, 0x37, 1, 0, 1, 0x1b, 1, 0, 4, 0x1b, 5, 0, 
        4, 0x1b, 1, 0, 4, 0x1b, 5, 0, 1, 0x1b, 1, 0x1c, 5, 0, 1, 0x2d, 
        1, 0x38, 1, 0, 1, 0x2d, 1, 0, 4, 0x2d, 5, 0, 4, 0x2d, 1, 0, 
        4, 0x2d, 5, 0, 1, 0x2d, 6, 0, 1, 0x1b, 1, 0x37, 1, 0, 1, 0x1b, 
        1, 0, 3, 0x1b, 1, 0x39, 5, 0, 4, 0x1b, 1, 0, 4, 0x1b, 5, 0, 
        1, 0x1b, 1, 0x1c, 0x11, 0, 1, 0x3a, 0x16, 0, 1, 0x3b, 5, 0, 3, 0x3b, 
        5, 0, 4, 0x3b, 1, 0, 3, 0x3b, 13, 0, 1, 0x3b, 5, 0, 1, 0x3b, 
        1, 60, 1, 0x3b, 5, 0, 4, 0x3b, 1, 0, 3, 0x3b, 0x1c, 0, 1, 0x3d, 
        0x27, 0, 1, 0x3e, 0x10, 0, 1, 0x3f, 0x2c, 0, 1, 0x40, 0x16, 0, 2, 0x41, 
        1, 0x42, 1, 0x41, 1, 0, 4, 0x41, 5, 0, 4, 0x41, 1, 0, 3, 0x41, 
        1, 0x1b, 5, 0, 1, 0x1b, 1, 0x1c, 5, 0, 1, 0x43, 1, 0x38, 1, 0, 
        1, 0x2d, 1, 0, 1, 0x2d, 3, 0x43, 5, 0, 4, 0x43, 1, 0, 3, 0x43, 
        1, 0x2d, 5, 0, 1, 0x43, 6, 0, 2, 0x1b, 1, 0, 1, 0x1b, 1, 0, 
        4, 0x1b, 1, 0x44, 4, 0, 4, 0x1b, 1, 0, 4, 0x1b, 5, 0, 1, 0x1b, 
        1, 0x1c, 0x11, 0, 1, 0x45, 0x17, 0, 1, 70, 0x22, 0, 1, 70, 6, 0, 
        1, 0x47, 0x2a, 0, 1, 0x48, 0x27, 0, 1, 0x49, 10, 0, 1, 0x3f, 1, 0x4a, 
        1, 0x4b, 2, 0x3f, 1, 0, 0x1d, 0x3f, 15, 0, 1, 0x4c, 0x16, 0, 1, 0x41, 
        1, 0x4d, 1, 0x42, 1, 0x41, 1, 0, 4, 0x41, 5, 0, 4, 0x41, 1, 0, 
        3, 0x41, 1, 0x1b, 5, 0, 1, 0x1b, 1, 0x1c, 5, 0, 1, 0x42, 1, 0x4e, 
        2, 0x42, 1, 0, 4, 0x42, 5, 0, 4, 0x42, 1, 0, 3, 0x42, 13, 0, 
        1, 0x4f, 1, 0x38, 1, 0, 1, 0x2d, 1, 0, 1, 0x2d, 3, 0x4f, 5, 0, 
        4, 0x4f, 1, 0, 3, 0x4f, 1, 0x2d, 5, 0, 1, 0x4f, 10, 0, 1, 80, 
        30, 0, 4, 0x51, 1, 0, 4, 0x51, 5, 0, 4, 0x51, 1, 0, 3, 0x51, 
        0x16, 0, 1, 0x52, 0x2a, 0, 1, 0x53, 0x1a, 0, 1, 0x54, 0x18, 0, 1, 0x4b, 
        0x23, 0, 1, 0x55, 1, 0x4d, 1, 0x42, 1, 0x55, 1, 0, 1, 0x41, 3, 0x55, 
        5, 0, 4, 0x55, 1, 0, 3, 0x55, 1, 0x1b, 5, 0, 1, 0x1b, 1, 0x1c, 
        5, 0, 1, 0x56, 1, 0x4e, 1, 0x42, 1, 0x56, 1, 0, 1, 0x42, 3, 0x56, 
        5, 0, 4, 0x56, 1, 0, 3, 0x56, 13, 0, 1, 0x57, 1, 0x38, 1, 0, 
        1, 0x2d, 1, 0, 1, 0x2d, 3, 0x57, 5, 0, 4, 0x57, 1, 0, 3, 0x57, 
        1, 0x2d, 5, 0, 1, 0x57, 10, 0, 1, 0x58, 30, 0, 1, 0x51, 1, 0x59, 
        2, 0x51, 1, 0, 4, 0x51, 5, 0, 4, 0x51, 1, 0, 3, 0x51, 0x11, 0, 
        1, 90, 0x27, 0, 1, 0x5b, 0x1c, 0, 1, 0x5c, 0x12, 0, 1, 0x5c, 12, 0, 
        1, 0x5d, 1, 0x4d, 1, 0x42, 1, 0x5d, 1, 0, 1, 0x41, 3, 0x5d, 5, 0, 
        4, 0x5d, 1, 0, 3, 0x5d, 1, 0x1b, 5, 0, 1, 0x1b, 1, 0x1c, 5, 0, 
        1, 0x5e, 1, 0x4e, 1, 0x42, 1, 0x5e, 1, 0, 1, 0x42, 3, 0x5e, 5, 0, 
        4, 0x5e, 1, 0, 3, 0x5e, 13, 0, 1, 0x5f, 2, 0, 1, 0x60, 2, 0, 
        3, 0x5f, 5, 0, 4, 0x5f, 1, 0, 3, 0x5f, 13, 0, 1, 0x61, 1, 0x59, 
        1, 0x51, 1, 0x61, 1, 0, 1, 0x51, 3, 0x61, 5, 0, 4, 0x61, 1, 0, 
        3, 0x61, 0x11, 0, 1, 0x62, 0x1b, 0, 15, 0x63, 1, 0, 5, 0x63, 1, 0, 
        13, 0x63, 6, 0, 1, 0x5c, 14, 0, 1, 100, 3, 0, 1, 0x5c, 1, 0x65, 
        11, 0, 2, 0x5d, 1, 0x42, 1, 0x5d, 1, 0x66, 4, 0x5d, 5, 0, 4, 0x5d, 
        1, 0, 3, 0x5d, 1, 0x1b, 5, 0, 1, 0x1b, 1, 0x1c, 5, 0, 2, 0x5e, 
        1, 0x42, 1, 0x5e, 1, 0x66, 4, 0x5e, 5, 0, 4, 0x5e, 1, 0, 3, 0x5e, 
        13, 0, 1, 0x67, 5, 0, 3, 0x67, 5, 0, 4, 0x67, 1, 0, 3, 0x67, 
        14, 0, 1, 0x68, 1, 0, 1, 0x69, 0x1f, 0, 1, 0x6a, 1, 0x59, 1, 0x51, 
        1, 0x6a, 1, 0, 1, 0x51, 3, 0x6a, 5, 0, 4, 0x6a, 1, 0, 3, 0x6a, 
        13, 0, 1, 0x20, 2, 0, 1, 0x6b, 2, 0, 3, 0x20, 5, 0, 4, 0x20, 
        1, 0, 3, 0x20, 10, 0, 15, 0x63, 1, 0, 5, 0x63, 1, 0x6c, 13, 0x63, 
        0x15, 0, 1, 0x6d, 0x13, 0, 1, 110, 0x12, 0, 1, 110, 12, 0, 2, 0x66, 
        1, 0, 6, 0x66, 5, 0, 4, 0x66, 1, 0, 3, 0x66, 13, 0, 1, 0x6f, 
        5, 0, 3, 0x6f, 5, 0, 4, 0x6f, 1, 0, 3, 0x6f, 0x10, 0, 1, 0x70, 
        0x20, 0, 1, 0x68, 1, 0, 1, 0x71, 0x1f, 0, 2, 0x6a, 1, 0x51, 1, 0x6a, 
        1, 0x72, 4, 0x6a, 5, 0, 4, 0x6a, 1, 0, 3, 0x6a, 3, 0, 1, 0x73, 
        10, 0, 1, 0x74, 1, 0, 1, 0x75, 0x31, 0, 1, 0x76, 0x13, 0, 1, 110, 
        14, 0, 1, 0x77, 3, 0, 1, 110, 13, 0, 1, 120, 0x22, 0, 1, 0x79, 
        1, 0, 1, 0x7a, 0x20, 0, 1, 0x68, 0x21, 0, 2, 0x72, 1, 0, 6, 0x72, 
        5, 0, 4, 0x72, 1, 0, 3, 0x72, 3, 0, 1, 0x73, 6, 0, 1, 0x7b, 
        2, 0, 0x12, 0x7b, 1, 0, 6, 0x7b, 1, 0, 1, 0x7b, 1, 0, 4, 0x7b, 
        6, 0, 1, 0x7c, 0x20, 0, 1, 0x74, 1, 0, 1, 0x7d, 0x31, 0, 1, 0x7e, 
        0x10, 0, 4, 0x42, 1, 0, 4, 0x42, 5, 0, 4, 0x42, 1, 0, 3, 0x42, 
        0x10, 0, 1, 0x7f, 0x20, 0, 1, 0x79, 1, 0, 1, 0x80, 0x1c, 0, 1, 0x7b, 
        2, 0, 0x12, 0x7b, 1, 0x81, 6, 0x7b, 1, 0, 1, 0x7b, 1, 0, 4, 0x7b, 
        4, 0, 1, 130, 1, 0, 1, 0x83, 0x20, 0, 1, 0x74, 0x22, 0, 1, 0x84, 
        1, 0, 1, 0x85, 0x20, 0, 1, 0x79, 0x24, 0, 1, 0x86, 0x20, 0, 1, 130, 
        1, 0, 1, 0x87, 0x22, 0, 1, 0x66, 0x20, 0, 1, 0x84, 1, 0, 1, 0x88, 
        0x20, 0, 1, 0x89, 1, 0, 1, 0x8a, 0x20, 0, 1, 130, 0x22, 0, 1, 0x84, 
        0x24, 0, 1, 0x72, 0x20, 0, 1, 0x89, 1, 0, 1, 0x8b, 0x20, 0, 1, 0x89, 
        30, 0, 0
     };
        private const int ZZ_UNKNOWN_ERROR = 0;
        private bool zzAtBOL;
        private bool zzAtEOF;
        private char[] zzBuffer;
        private int zzCurrentPos;
        private int zzEndRead;
        private int zzLexicalState;
        private int zzMarkedPos;
        private int zzPushbackPos;
        private TextReader zzReader;
        private int zzStartRead;
        private int zzState;

        // Methods
        internal Lexer(Stream @in)
            : this(new StreamReader(@in))
        {
        }

        internal Lexer(TextReader @in)
        {
            this.zzBuffer = new char[0x4000];
            this.zzAtBOL = true;
            this.zzReader = @in;
        }

        public bool isZzAtEOF()
        {
            return this.zzAtEOF;
        }

        private Yytoken symbol(int type)
        {
            return new Yytoken(type, this.yyline, this.yycolumn);
        }

        private Yytoken symbol(int type, object value)
        {
            return new Yytoken(type, this.yyline, this.yycolumn, value);
        }

        public void yybegin(int newState)
        {
            this.zzLexicalState = newState;
        }

        public char yycharat(int pos)
        {
            return this.zzBuffer[this.zzStartRead + pos];
        }

        public void yyclose()
        {
            this.zzAtEOF = true;
            this.zzEndRead = this.zzStartRead;
            if (this.zzReader != null)
            {
                this.zzReader.Close();
            }
        }

        public int yylength()
        {
            return (this.zzMarkedPos - this.zzStartRead);
        }

        public Yytoken yylex()
        {
            int zzState;
            int zzCurrentPos;
            int zzMarkedPos;
            int index = 0;
            int zzEndRead = this.zzEndRead;
            char[] zzBuffer = this.zzBuffer;
            char[] chArray2 = ZZ_CMAP;
            int[] numArray = ZZ_TRANS;
            int[] numArray2 = ZZ_ROWMAP;
            int[] numArray3 = ZZ_ATTRIBUTE;
        Label_002E:
            zzMarkedPos = this.zzMarkedPos;
            bool flag = false;
            for (zzCurrentPos = this.zzStartRead; zzCurrentPos < zzMarkedPos; zzCurrentPos++)
            {
                switch (zzBuffer[zzCurrentPos])
                {
                    case '\n':
                        {
                            if (!flag)
                            {
                                break;
                            }
                            flag = false;
                            continue;
                        }
                    case '\v':
                    case '\f':
                    case '\x0085':
                    case '\u2028':
                    case '\u2029':
                        {
                            this.yyline++;
                            this.yycolumn = 0;
                            flag = false;
                            continue;
                        }
                    case '\r':
                        {
                            this.yyline++;
                            this.yycolumn = 0;
                            flag = true;
                            continue;
                        }
                    default:
                        goto Label_00D8;
                }
                this.yyline++;
                this.yycolumn = 0;
                continue;
            Label_00D8:
                flag = false;
                this.yycolumn++;
            }
            if (flag)
            {
                bool flag2;
                if (zzMarkedPos < zzEndRead)
                {
                    flag2 = zzBuffer[zzMarkedPos] == '\n';
                }
                else if (this.zzAtEOF)
                {
                    flag2 = false;
                }
                else
                {
                    bool flag3 = this.zzRefill();
                    zzMarkedPos = this.zzMarkedPos;
                    zzBuffer = this.zzBuffer;
                    if (flag3)
                    {
                        flag2 = false;
                    }
                    else
                    {
                        flag2 = zzBuffer[zzMarkedPos] == '\n';
                    }
                }
                if (flag2)
                {
                    this.yyline--;
                }
            }
            if (zzMarkedPos > this.zzStartRead)
            {
                switch (zzBuffer[zzMarkedPos - 1])
                {
                    case '\n':
                    case '\v':
                    case '\f':
                    case '\x0085':
                    case '\u2028':
                    case '\u2029':
                        this.zzAtBOL = true;
                        goto Label_0210;

                    case '\r':
                        if (zzMarkedPos < zzEndRead)
                        {
                            this.zzAtBOL = zzBuffer[zzMarkedPos] != '\n';
                        }
                        else if (this.zzAtEOF)
                        {
                            this.zzAtBOL = false;
                        }
                        else
                        {
                            bool flag4 = this.zzRefill();
                            zzMarkedPos = this.zzMarkedPos;
                            zzBuffer = this.zzBuffer;
                            if (flag4)
                            {
                                this.zzAtBOL = false;
                            }
                            else
                            {
                                this.zzAtBOL = zzBuffer[zzMarkedPos] != '\n';
                            }
                        }
                        goto Label_0210;
                }
                this.zzAtBOL = false;
            }
        Label_0210:
            zzState = -1;
            zzCurrentPos = this.zzCurrentPos = this.zzStartRead = zzMarkedPos;
            if (this.zzAtBOL)
            {
                this.zzState = ZZ_LEXSTATE[this.zzLexicalState + 1];
            }
            else
            {
                this.zzState = ZZ_LEXSTATE[this.zzLexicalState];
            }
        Label_025A:
            if (ZZ_SPURIOUS_WARNINGS_SUCK)
            {
                if (zzCurrentPos < zzEndRead)
                {
                    index = zzBuffer[zzCurrentPos++];
                }
                else
                {
                    if (this.zzAtEOF)
                    {
                        index = -1;
                        goto Label_0310;
                    }
                    this.zzCurrentPos = zzCurrentPos;
                    this.zzMarkedPos = zzMarkedPos;
                    bool flag5 = this.zzRefill();
                    zzCurrentPos = this.zzCurrentPos;
                    zzMarkedPos = this.zzMarkedPos;
                    zzBuffer = this.zzBuffer;
                    zzEndRead = this.zzEndRead;
                    if (flag5)
                    {
                        index = -1;
                        goto Label_0310;
                    }
                    index = zzBuffer[zzCurrentPos++];
                }
                int num6 = numArray[numArray2[this.zzState] + chArray2[index]];
                if (num6 != -1)
                {
                    this.zzState = num6;
                    int num7 = numArray3[this.zzState];
                    if ((num7 & 1) != 1)
                    {
                        goto Label_025A;
                    }
                    zzState = this.zzState;
                    zzMarkedPos = zzCurrentPos;
                    if ((num7 & 8) != 8)
                    {
                        goto Label_025A;
                    }
                }
            }
        Label_0310:
            this.zzMarkedPos = zzMarkedPos;
            switch (((zzState < 0) ? zzState : ZZ_ACTION[zzState]))
            {
                case 1:
                    if (!ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                        goto Label_002E;
                    }
                    return this.symbol(10, this.yytext());

                case 2:
                    if (!ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                        goto Label_002E;
                    }
                    return this.symbol(8, this.yytext());

                case 3:
                    if (!ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                        goto Label_002E;
                    }
                    return this.symbol(4, this.yytext().Length);

                case 4:
                    if (!ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                        goto Label_002E;
                    }
                    return this.symbol(2, this.yytext().Length);

                case 5:
                    if (!ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                        goto Label_002E;
                    }
                    return this.symbol(3, this.yytext().Length);

                case 6:
                    if (ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                        this.headingManager = new HeadingManager(this.yytext());
                        this.yybegin(2);
                    }
                    goto Label_002E;

                case 7:
                    if (ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                        Console.WriteLine("H<" + this.yytext() + ">");
                        this.headingManager.append(this.yytext());
                    }
                    goto Label_002E;

                case 8:
                    if (!ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                        goto Label_002E;
                    }
                    this.headingManager.flush();
                    this.yybegin(0);
                    return this.symbol(12, this.headingManager);

                case 9:
                    if (!ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                        goto Label_002E;
                    }
                    return this.symbol(11);

                case 10:
                    if (!ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                        goto Label_002E;
                    }
                    return this.symbol(6);

                case 11:
                    if (!ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                    }
                    goto Label_002E;

                case 12:
                    if (!ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                        goto Label_002E;
                    }
                    return this.symbol(0x11);

                case 13:
                    if (!ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                        goto Label_002E;
                    }
                    return this.symbol(0x12);

                case 14:
                    if (!ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                        goto Label_002E;
                    }
                    return this.symbol(0x13);

                case 15:
                    if (ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                        this.headingManager.setUnderline();
                    }
                    goto Label_002E;

                case 0x10:
                    if (ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                        this.headingManager.setItalic();
                    }
                    goto Label_002E;

                case 0x11:
                    if (!ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                        goto Label_002E;
                    }
                    return this.symbol(5);

                case 0x12:
                    if (ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                        this.headingManager.setBold();
                    }
                    goto Label_002E;

                case 0x13:
                    if (!ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                        goto Label_002E;
                    }
                    return this.symbol(7);

                case 20:
                    if (!ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                        goto Label_002E;
                    }
                    return this.symbol(0x10);

                case 0x15:
                    if (ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                        this.headingManager.setBoldItalic();
                    }
                    goto Label_002E;

                case 0x16:
                    if (!ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                        goto Label_002E;
                    }
                    return this.symbol(15, this.yytext());

                case 0x17:
                    if (!ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                        goto Label_002E;
                    }
                    return this.symbol(13, this.yytext());

                case 0x18:
                    if (!ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                        goto Label_002E;
                    }
                    return this.symbol(20, this.yytext());

                case 0x19:
                    if (!ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                        goto Label_002E;
                    }
                    return this.symbol(1, this.yytext());

                case 0x1a:
                    if (!ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                        goto Label_002E;
                    }
                    return this.symbol(20, this.yytext());

                case 0x1b:
                    if (!ZZ_SPURIOUS_WARNINGS_SUCK)
                    {
                        goto Label_002E;
                    }
                    return this.symbol(14, this.yytext());
            }
            if ((index == -1) && (this.zzStartRead == this.zzCurrentPos))
            {
                this.zzAtEOF = true;
                return null;
            }
            this.zzScanError(1);
            goto Label_002E;
        }

        public void yypushback(int number)
        {
            if (number > this.yylength())
            {
                this.zzScanError(2);
            }
            this.zzMarkedPos -= number;
        }

        public void yyreset(TextReader reader)
        {
            this.zzReader = reader;
            this.zzAtBOL = true;
            this.zzAtEOF = false;
            this.zzEndRead = this.zzStartRead = 0;
            this.zzCurrentPos = this.zzMarkedPos = this.zzPushbackPos = 0;
            this.yyline = this.yychar = this.yycolumn = 0;
            this.zzLexicalState = 0;
        }

        public int yystate()
        {
            return this.zzLexicalState;
        }

        public string yytext()
        {
            return new string(this.zzBuffer, this.zzStartRead, this.zzMarkedPos - this.zzStartRead);
        }

        private bool zzRefill()
        {
            if (this.zzStartRead > 0)
            {
                Array.Copy(this.zzBuffer, this.zzStartRead, this.zzBuffer, 0, this.zzEndRead - this.zzStartRead);
                this.zzEndRead -= this.zzStartRead;
                this.zzCurrentPos -= this.zzStartRead;
                this.zzMarkedPos -= this.zzStartRead;
                this.zzPushbackPos -= this.zzStartRead;
                this.zzStartRead = 0;
            }
            if (this.zzCurrentPos >= this.zzBuffer.Length)
            {
                char[] destinationArray = new char[this.zzCurrentPos * 2];
                Array.Copy(this.zzBuffer, 0, destinationArray, 0, this.zzBuffer.Length);
                this.zzBuffer = destinationArray;
            }
            int num = this.zzReader.Read(this.zzBuffer, this.zzEndRead, this.zzBuffer.Length - this.zzEndRead);
            if (num <= 0)
            {
                return true;
            }
            this.zzEndRead += num;
            return false;
        }

        private void zzScanError(int errorCode)
        {
            string str;
            try
            {
                str = ZZ_ERROR_MSG[errorCode];
            }
            catch (IndexOutOfRangeException)
            {
                str = ZZ_ERROR_MSG[0];
            }
            throw new Exception(str);
        }

        private static int[] zzUnpackAction()
        {
            int[] result = new int[0x8b];
            int offset = 0;
            offset = zzUnpackAction(ZZ_ACTION_PACKED_0, offset, result);
            return result;
        }

        private static int zzUnpackAction(ushort[] packed, int offset, int[] result)
        {
            int num = 0;
            int num2 = offset;
            int length = packed.Length;
            while ((num + 1) < length)
            {
                int num4 = packed[num++];
                int num5 = packed[num++];
                do
                {
                    result[num2++] = num5;
                }
                while (--num4 > 0);
            }
            return num2;
        }

        private static int[] zzUnpackAttribute()
        {
            int[] result = new int[0x8b];
            int offset = 0;
            offset = zzUnpackAttribute(ZZ_ATTRIBUTE_PACKED_0, offset, result);
            return result;
        }

        private static int zzUnpackAttribute(ushort[] packed, int offset, int[] result)
        {
            int num = 0;
            int num2 = offset;
            int length = packed.Length;
            while ((num + 1) < length)
            {
                int num4 = packed[num++];
                int num5 = packed[num++];
                do
                {
                    result[num2++] = num5;
                }
                while (--num4 > 0);
            }
            return num2;
        }

        private static char[] zzUnpackCMap(ushort[] packed)
        {
            char[] chArray = new char[0x10000];
            int num = 0;
            int num2 = 0;
            while (num < 0x4aa)
            {
                int num3 = packed[num++];
                char ch = (char)packed[num++];
                do
                {
                    chArray[num2++] = ch;
                }
                while (--num3 > 0);
            }
            return chArray;
        }

        private static int[] zzUnpackRowMap()
        {
            int[] result = new int[0x8b];
            int offset = 0;
            offset = zzUnpackRowMap(ZZ_ROWMAP_PACKED_0, offset, result);
            return result;
        }

        private static int zzUnpackRowMap(ushort[] packed, int offset, int[] result)
        {
            int num = 0;
            int num2 = offset;
            int length = packed.Length;
            while ((num + 1) < length)
            {
                int num4 = packed[num++] << 0x10;
                result[num2++] = num4 | packed[num++];
            }
            return num2;
        }

        private static int[] zzUnpackTrans()
        {
            int[] result = new int[0x1068];
            int offset = 0;
            offset = zzUnpackTrans(ZZ_TRANS_PACKED_0, offset, result);
            return result;
        }

        private static int zzUnpackTrans(ushort[] packed, int offset, int[] result)
        {
            int num = 0;
            int num2 = offset;
            int length = packed.Length;
            while ((num + 1) < length)
            {
                int num4 = packed[num++];
                int num5 = packed[num++];
                num5--;
                do
                {
                    result[num2++] = num5;
                }
                while (--num4 > 0);
            }
            return num2;
        }
    }
}
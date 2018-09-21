using System.Collections;
using System.Text.RegularExpressions;

namespace ch.appl.psoft.Morph.lexer
{
    public class HtmlToCharMapper
    {
        // Fields
        private static Hashtable hash = new Hashtable();

        // Methods
        static HtmlToCharMapper()
        {
            hash.Add("&#160;", " ");
            hash.Add("&#161;", "\x00a1");
            hash.Add("&#162;", "\x00a2");
            hash.Add("&#163;", "\x00a3");
            hash.Add("&#164;", "\x00a4");
            hash.Add("&#165;", "\x00a5");
            hash.Add("&#166;", "\x00a6");
            hash.Add("&#167;", "\x00a7");
            hash.Add("&#168;", "\x00a8");
            hash.Add("&#169;", "\x00a9");
            hash.Add("&#170;", "\x00aa");
            hash.Add("&#171;", "\x00ab");
            hash.Add("&#172;", "\x00ac");
            hash.Add("&#173;", "\x00ad");
            hash.Add("&#174;", "\x00ae");
            hash.Add("&#175;", "\x00af");
            hash.Add("&#176;", "\x00b0");
            hash.Add("&#177;", "\x00b1");
            hash.Add("&#178;", "\x00b2");
            hash.Add("&#179;", "\x00b3");
            hash.Add("&#180;", "\x00b4");
            hash.Add("&#181;", "\x00b5");
            hash.Add("&#182;", "\x00b6");
            hash.Add("&#183;", "\x00b7");
            hash.Add("&#184;", "\x00b8");
            hash.Add("&#185;", "\x00b9");
            hash.Add("&#186;", "\x00ba");
            hash.Add("&#187;", "\x00bb");
            hash.Add("&#188;", "\x00bc");
            hash.Add("&#189;", "\x00bd");
            hash.Add("&#190;", "\x00be");
            hash.Add("&#191;", "\x00bf");
            hash.Add("&#192;", "\x00c0");
            hash.Add("&#193;", "\x00c1");
            hash.Add("&#194;", "\x00c2");
            hash.Add("&#195;", "\x00c3");
            hash.Add("&#197;", "\x00c5");
            hash.Add("&#198;", "\x00c6");
            hash.Add("&#199;", "\x00c7");
            hash.Add("&#200;", "\x00c8");
            hash.Add("&#201;", "\x00c9");
            hash.Add("&#202;", "\x00ca");
            hash.Add("&#203;", "\x00cb");
            hash.Add("&#204;", "\x00cc");
            hash.Add("&#205;", "\x00cd");
            hash.Add("&#206;", "\x00ce");
            hash.Add("&#207;", "\x00cf");
            hash.Add("&#208;", "\x00d0");
            hash.Add("&#209;", "\x00d1");
            hash.Add("&#210;", "\x00d2");
            hash.Add("&#211;", "\x00d3");
            hash.Add("&#212;", "\x00d4");
            hash.Add("&#213;", "\x00d5");
            hash.Add("&#214;", "\x00d6");
            hash.Add("&#215;", "\x00d7");
            hash.Add("&#216;", "\x00d8");
            hash.Add("&#217;", "\x00d9");
            hash.Add("&#218;", "\x00da");
            hash.Add("&#219;", "\x00db");
            hash.Add("&#220;", "\x00dc");
            hash.Add("&#221;", "\x00dd");
            hash.Add("&#222;", "\x00de");
            hash.Add("&#223;", "\x00df");
            hash.Add("&#224;", "\x00e0");
            hash.Add("&#225;", "\x00e1");
            hash.Add("&#226;", "\x00e2");
            hash.Add("&#227;", "\x00e3");
            hash.Add("&#228;", "\x00e4");
            hash.Add("&#229;", "\x00e5");
            hash.Add("&#230;", "\x00e6");
            hash.Add("&#231;", "\x00e7");
            hash.Add("&#232;", "\x00e8");
            hash.Add("&#233;", "\x00e9");
            hash.Add("&#234;", "\x00ea");
            hash.Add("&#235;", "\x00eb");
            hash.Add("&#236;", "\x00ec");
            hash.Add("&#237;", "\x00ed");
            hash.Add("&#238;", "\x00ee");
            hash.Add("&#239;", "\x00ef");
            hash.Add("&#240;", "\x00f0");
            hash.Add("&#241;", "\x00f1");
            hash.Add("&#242;", "\x00f2");
            hash.Add("&#243;", "\x00f3");
            hash.Add("&#244;", "\x00f4");
            hash.Add("&#246;", "\x00f6");
            hash.Add("&#247;", "\x00f7");
            hash.Add("&#248;", "\x00f8");
            hash.Add("&#249;", "\x00f9");
            hash.Add("&#250;", "\x00fa");
            hash.Add("&#251;", "\x00fb");
            hash.Add("&#252;", "\x00fc");
            hash.Add("&#253;", "\x00fd");
            hash.Add("&#254;", "\x00fe");
            hash.Add("&#255;", "\x00ff");
        }

        public static string convert(string input)
        {
            string str = input;
            for (Match match = Regex.Match(input, "&#[1-9]{3};"); match.Success; match = match.NextMatch())
            {
                string oldValue = match.Value;
                string newValue = (string)hash[oldValue];
                if (newValue != null)
                {
                    str = str.Replace(oldValue, newValue);
                }
            }
            return str;
        }
    }
}
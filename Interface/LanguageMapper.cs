using ch.psoft.Util;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.Interface
{
    /// <summary>
    /// Bildet Mnemonics sprachabhaengig auf den entsprechenden Text ab
    /// </summary>
    public class LanguageMapper {
        private static char[] ZERO = {(char) 0};
        private static string SEP = new String(ZERO);
        private static string ALIAS = "#alias#";
        private Hashtable table = new Hashtable(); 
        private String _languageCode = "";
        protected const string _languageMapper = "LangMap";
        
        internal class TextContext {
            internal int w = -1;
            internal int h = -1;
            internal string text = "";
            internal string mnemo = "";
            
            internal TextContext (string mnemo,int w, int h) {
                this.w = w;
                this.h = h;
                this.mnemo = mnemo;
            }
            public override string ToString() {
                return this.text;
            }
            public override int GetHashCode() {
                return this.w.GetHashCode()+this.h.GetHashCode()+this.text.GetHashCode();
            }
            
        }

        public static void setLanguageMapper(HttpSessionState Session, LanguageMapper langMap) {
            Session[_languageMapper] = langMap;
        }

        public static LanguageMapper getLanguageMapper(HttpSessionState Session) {
            return (LanguageMapper) Session[_languageMapper];
        }

        public static void setLanguageMapper(HttpApplicationState Application, LanguageMapper langMap) {
            Application[_languageMapper] = langMap;
        }

        public static LanguageMapper getLanguageMapper(HttpApplicationState Application) {
            return (LanguageMapper) Application[_languageMapper];
        }


        /// <summary>
        /// Load language relevant text strings from the appropriate XML file. 
        /// </summary>
        /// <param name="fileName">Fully Qualified filename</param>
        /// <param name="languageCode">Language code like 'en', 'de', 'it' or 'fr'</param>
        /// <param name="clear">true: clear table before load</param>
        /// <returns>true: ok</returns>
        public bool load(String fileName, String languageCode, bool clear)
        {
             return load (fileName, languageCode, clear, false);
        }

        /// <summary>
        /// Load language relevant text strings from the appropriate XML file. 
        /// </summary>
        /// <param name="fileName">Fully Qualified filename</param>
        /// <param name="languageCode">Language code like 'en', 'de', 'it' or 'fr'</param>
        /// <param name="clear">true: clear table before load</param>
        /// <param name="overwrite">true: values are overwritten</param>
        /// <returns>true: ok</returns>
        public bool load (String fileName, String languageCode, bool clear, bool overwrite) {
            String mnemo = "";
            String scope = "",escope = "",alias = "";
            String id = "";
            String ename = "";
            bool ok = true, global = false;
            XmlReader reader = null;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;
            settings.DtdProcessing = DtdProcessing.Ignore;
            settings.ValidationType = ValidationType.DTD;

            ArrayList tokenList = null;
            TextContext propVal = new TextContext("",-1,-1), val;
             
            //Logger.LogAll();
            _languageCode = languageCode.ToLower();
            if (clear) this.table.Clear();
            try {           
                Logger.Log("Load language: " + fileName,Logger.DEBUG);
                using (reader = XmlReader.Create(fileName, settings))
                while (reader.Read()) {
                    try {
                        switch (reader.NodeType) {
                        case XmlNodeType.Text:
                            propVal.text = reader.Value;
                            break;
                        case XmlNodeType.Element:
                            switch (reader.Name) {
                            case "global":
                                scope = "";
                                escope = "";
                                global = true;
                                break;
                            case "local":
                                scope = reader.GetAttribute("scope") + SEP; 
                                escope = scope;
                                alias = ch.psoft.Util.Validate.GetValid(reader.GetAttribute("alias"));
                                break;
                            case "enum":
                                tokenList = new ArrayList();
                                ename = reader.GetAttribute("name");
                                escope = scope + ename + SEP;
                                break;
                            case "prop":
                                mnemo = reader.GetAttribute("mnemo");
                                string size = ch.psoft.Util.Validate.GetValid(reader.GetAttribute("size"),"-1,-1");
                                string[] wh = size.Split(',');
                                int w = wh.Length > 0 ? int.Parse(wh[0]) : -1;
                                int h = wh.Length > 1 ? int.Parse(wh[1]) : -1;
                                propVal = new TextContext(mnemo,w,h);
                                if (mnemo == ALIAS) throw new ArgumentException("Mnemonic '"+ALIAS+"' reserved !");
                                break;
                            default:
                                break;
                            }
                            break;

                        case XmlNodeType.EndElement:
                            switch (reader.Name) {
                            case "enum":
                                id = scope + ename;
                                Logger.Log("load enum: " + id.Replace(SEP,"^"),Logger.VERBOSE);
                                this.insert(id,tokenList, overwrite, false);
                                tokenList = null;
                                if (global) {
                                    scope = "";
                                    escope = "";
                                }
                                break;
                            case "prop":
                                id = escope + propVal.mnemo;
                                Logger.Log("load mnemo: "+id.Replace(SEP,"^")+"->"+propVal,Logger.VERBOSE);
                                this.insert(id, propVal, true, true); //second argument always true acccording to revision 566
                                if (tokenList != null) tokenList.Add(propVal.mnemo);
                                break;
                            case "local":
                                if (alias != "") {
                                    id = scope + ALIAS;
                                    val = new TextContext(ALIAS,-1,-1);
                                    val.text = alias;
                                    Logger.Log("load alias: "+id.Replace(SEP,"^")+"->"+val,Logger.VERBOSE);
                                    this.insert(id,val, overwrite, true);
                                }
                                break;
                            case "global":
                                global = false;
                                break;
                            default:
                                break;
                            }
                            break;

                        default:
                            break;
                        }
                    }
                    catch (ArgumentException) {
                        Logger.Log("Key already exists in table, key = " + id.Replace(SEP,"^"),Logger.WARNING);
                    }
                    catch (Exception e) {
                        Logger.Log(e.Message,Logger.ERROR);
                        ok = false;
                    }
                }
            }

            catch (Exception e) {
                Logger.Log(e.Message,Logger.ERROR);
                ok = false;
            }
            finally {
                if (reader != null) reader.Close();
                reader = null;
            }
            return ok;

        }
        /// <summary>
        /// Set size for a global scope mnemonic. 
        /// </summary>
        /// <param name="mnemonic">mnemonic</param>
        /// <param name="w">width</param>
        /// <param name="h">height</param>
        /// <param name="defaultW">default width</param>
        /// <param name="defaultH">default height</param>
        public void getSize (string mnemonic, out int w, out int h, params int[] defaults) {
            getSize("",mnemonic,out w, out h, defaults);
        }
        /// <summary>
        /// Get size for a scope relevant mnemonic.
        /// If no mapping exist, the function tries to search the mnemonic in the global mapping scope.
        /// </summary>
        /// <param name="scope">scope like pagename etc.</param>
        /// <param name="mnemonic">mnemonic</param>
        /// <param name="w">width</param>
        /// <param name="h">height</param>
        /// <param name="defaultW">default width</param>
        /// <param name="defaultH">default height</param>
        public void getSize (string scope, string mnemonic, out int w, out int h, params int[] defaults) {
            TextContext ctx = getCtx(scope,mnemonic);
            
            w = defaults.Length > 0 ? defaults[0] : -1;
            h = defaults.Length > 1 ? defaults[1] : -1;
            if (ctx != null) {
                w = (ctx.w < 0 ? w : ctx.w);
                h = (ctx.h < 0 ? h : ctx.h);
            }
        }
        
        /// <summary>
        /// Get mapping for a global scope mnemonic. 
        /// </summary>
        /// <param name="mnemonic">Mnemonic</param>
        /// <returns>language specific text or mnemonic itself (if no mapping exist)</returns>
        public string get (String mnemonic) {
            return get("",mnemonic);
        }
        /// <summary>
        /// Get string value for a scope relevant mnemonic.
        /// If no mapping exist, the function tries to search the mnemonic in the global mapping scope.
        /// </summary>
        /// <param name="scope">scope like pagename etc.</param>
        /// <param name="mnemonic">mnemonic</param>
        /// <returns>language specific text</returns>
        public string get (string scope, string mnemonic) {
            TextContext ctx = getCtx(scope,mnemonic);
            
            return ctx == null ? mnemonic : ctx.text;
        }

        public static string get(HttpSessionState session, string scope, string mnemonic)
        {
            return getLanguageMapper(session).get(scope, mnemonic);
        }

        public static string get(HttpSessionState session, string mnemonic)
        {
            return getLanguageMapper(session).get(mnemonic);
        }

        private TextContext getCtx (string scope,string mnemonic) {
            string val = scope == "" ? mnemonic : scope + SEP + mnemonic;
            TextContext ctx = (TextContext) this.table [val];

            
            if (ctx == null) {
                if (scope == "") return null;
                val = scope + SEP + ALIAS;
                ctx = (TextContext) this.table [val];
                return ctx == null ? getCtx ("",mnemonic) : getCtx (ctx.text,mnemonic);
            }
            return ctx;

        }
        private string get (String scope1,String scope2,String mnemonic) {
            string val = scope1 + SEP + scope2 + SEP + mnemonic;
            TextContext ctx = (TextContext)  this.table [val];
            
            if (ctx == null) return get(scope1,mnemonic);
            else return ctx.text;

        }

        /// <summary>
        /// Get mapping for enummeration.
        /// Scope rule to find mnemonic:
        /// 1. enumscope
        /// 2. enumname
        /// </summary>
        /// <param name="scope">Enum scope</param>
        /// <param name="name">Enum name</param>
        /// <param name="mapIndex">True: map mnemonic to array index</param>
        /// <returns>array of language specific strings</returns>
        public String[] getEnum (String scope, String name, bool mapIndex ) {
            String val = scope + SEP + name;
            ArrayList tokens = (ArrayList) this.table [val];
            String[] enums;
            int maxIdx = -1;
            int idx = 0;

            if (tokens == null) return getEnum (name,mapIndex);

            enums = new String[tokens.Count];
            for (int i = 0; i < tokens.Count; i++) {
                enums[i]= get (scope,name,(String) tokens[i]);
                if (mapIndex) {
                    idx = Int32.Parse((String) tokens[i]);
                    if (idx > maxIdx) maxIdx = idx;
                }
            }
            if (mapIndex) return mapIdx(tokens,enums,maxIdx);
            else return enums;
        }

        /// <summary>
        /// Get mapping for global enummearation.
        /// Scope rule to find mnemonic:
        /// 1. enumname
        /// 2. global
        /// </summary>
        /// <param name="name">Enum name</param>
        /// <param name="mapIndex">True: map mnemonic to array-index</param>
        /// <returns>array of language specific string</returns>
        public String[] getEnum (String name, bool mapIndex) {
            String val = name;
            ArrayList tokens = (ArrayList) this.table [val];
            String[] enums;
            int maxIdx = -1;
            int idx = 0;

            if (tokens == null) return new String[] {name};

            enums = new String[tokens.Count];
            for (int i = 0; i < tokens.Count; i++) {
                enums[i] = get (name,(String) tokens[i]);
                if (mapIndex) {
                    idx = Int32.Parse((String) tokens[i]);
                    if (idx > maxIdx) maxIdx = idx;
                }
            }
            if (mapIndex) return mapIdx(tokens,enums,maxIdx);
            else return enums;
        }
        
        /// <summary>
        /// Get mnemonic and mapping for enummeration.
        /// Scope rule to find mnemonic:
        /// 1. enumscope
        /// 2. enumname
        /// </summary>
        /// <param name="scope">Enum scope</param>
        /// <param name="name">Enum name</param>
        /// <returns>2-Dimensional array of language specific menemonics (index [0,n]) and texts (index [1,n]</returns>
        public string[,] getEnum (string scope, string name) {
            string val = scope + SEP + name;
            ArrayList tokens = (ArrayList) this.table[val];
            string[,] enums;

            if (tokens == null){
                tokens = (ArrayList) this.table[name];
            }
            if (tokens == null){
                return new string[0,0];
            }

            enums = new string[2, tokens.Count];
            for (int i=0; i<tokens.Count; i++) {
                enums[0,i] = (string) tokens[i];
                enums[1,i] = get(scope, name, (string) tokens[i]);
            }
            return enums;
        }

        /// <summary>
        /// Get a single enumeration token
        /// </summary>
        /// <param name="scope">Enum scope</param>
        /// <param name="name">Enum name</param>
        /// <param name="mnemonic">Token mnemonic</param>
        public String getToken(String scope, String name, String mnemonic) {
            String val = scope + SEP + name + SEP + mnemonic;
            TextContext token = (TextContext) this.table [val];

            if (token == null) {
                val = name + SEP + mnemonic;
                token = (TextContext) this.table [val];
                if (token == null) return mnemonic;
            }
            return token.text;
        }
        /// <summary>
        /// Get a single enumeration token
        /// </summary>
        /// <param name="name">Enum name</param>
        /// <param name="mnemonic">Token mnemonic</param>
        public String getToken(String name, String mnemonic) {
            String val = name + SEP + mnemonic;
            TextContext token = (TextContext) this.table [val];

            if (token == null) return mnemonic;
            return token.text;
        }
        /// <summary>
        /// Returns the language-code used.
        /// </summary>
        public String LanguageCode {
            get { return _languageCode; }
        }
        private String[] mapIdx(ArrayList tokens, String[] enums, int maxIdx) {
            String[] enumsMap = new String[maxIdx+1];
            int idx = 0;

            for (int i = 0; i < enumsMap.Length; i++) { enumsMap[i] = null; }
            for (int i = 0; i < enums.Length; i++) {
                idx = Int32.Parse((String) tokens[i]);
                enumsMap[idx] = enums[i];
            }
		
            return enumsMap;
        }

  

        /// <summary>
        /// Add elements into table. Loading customer specific XML files 
        /// default element values can be replaced by customer specific 
        /// ones (same key but different value).
        /// </summary>
        /// <param name="id">The key of the element</param>
        /// <param name="elm">The value od the given key (id), 
        /// if there is already a value it is overwritten</param>
        /// <param name="overwrite">if true the value is overwritten, 
        /// an exception is thrown otherwise.</param>
        /// <param name="tryUpdateDb">if true the UNIT field is updated 
        /// if necessary</param>
        private void insert(string id, object elm, bool overwrite, bool tryUpdateDb)
        {
            if (overwrite)
            {
                //check if the key is already used
                if (!this.table.ContainsKey(id))
                {
                    //newly added
                    this.table.Add(id, elm);
                }
                else
                {
                    if (tryUpdateDb)
                    {
                        //check if the value has a user defined unit which is 
                        //different than the default and update it in the DB
                        string text = ((TextContext)elm).text;
                        DeltaXmlTextParser parser = new DeltaXmlTextParser();
                        ((TextContext)elm).text = parser.parse(((TextContext)elm).text);
                        if (parser.Valid) //matched!
                        {
                            //update the db if it is a DB translation
                            tryToUpdateDb(id, text, parser);
                        }
                    }
                    //overwrite
                    this.table[id] = elm;                   
                }
            }
            else
            {
                //exception if already contained
                this.table.Add(id, elm);
            }

        }

        

     
        /// <summary>
        /// Update the UNIT field in the DESCRIBEOBJV view table. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="text"></param>
        /// <param name="unit"></param>
        private void tryToUpdateDb(string id, string text, DeltaXmlTextParser parsed)
        {
            if (parsed == null || !parsed.Valid) return;
            db.DBData db = ch.appl.psoft.db.DBData.getDBData();
            db.connect();
            try
            {
                string[] splitted = System.Text.RegularExpressions.Regex.Split(id, SEP);
                if (splitted.Length != 2) return;
                string objname = splitted[0];
                string colname = splitted[1];
                //test if the entry is available
                objname = db.lookup("objname", "describeobjv", "objname = '" + objname + "' and COLNAME = '" + colname + "'", "");
                if (objname != splitted[0]) return;

                //update UNIT field
                string sqlUpdateStatement = "update describeobjv set unit = " + parsed.Unit + ", minvalue = " + parsed.Min +
                                             ", maxvalue = " + parsed.Max + " where objname = '" + objname + "' and COLNAME = '" + colname + "'";
                db.beginTransaction();
                db.execute(sqlUpdateStatement);
                db.commit();
            }
            catch (Exception ex)
            {
                db.rollback();
                Logger.Log(ex, Logger.ERROR);
            }
            finally
            {
                db.disconnect();
            }
        }

        private class DeltaXmlTextParser
        {
            const string UNIT_KEYWORD = "unit=";
            const string RANGE_KEYWORD = "range=";

            /// <summary>
            /// returns the string without unit and range data.
            /// </summary>
            /// <param name="input"></param>
            /// <returns></returns>
            public string parse(string input)
            {
                Regex rx = new Regex(@"[ ]*{[^}]+}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                MatchCollection coll = rx.Matches(input);

                this.Valid = (coll.Count > 0);
                if (!this.Valid)
                {
                    return input;
                }

                int matchIndex = input.Length - 1;
                foreach (System.Text.RegularExpressions.Match mc in coll)
                {
                    string re = mc.Value.Substring(mc.Value.IndexOf("{") + 1, mc.Value.Length - mc.Value.IndexOf("{") - 2);

                    if (matchIndex > mc.Index) { matchIndex = mc.Index; }

                    int index = re.IndexOf(UNIT_KEYWORD);
                    if (index >= 0)
                    {
                        this.unit = re.Substring(index + UNIT_KEYWORD.Length, re.Length - index - UNIT_KEYWORD.Length);
                    }
                    else
                    {
                        index = re.IndexOf(RANGE_KEYWORD);
                        if (index < 0)
                        {
                            this.Valid = false;
                            return input;
                        }
                        string value = re.Substring(index + RANGE_KEYWORD.Length, re.Length - index - RANGE_KEYWORD.Length);
                        //min
                        Regex rrange = new Regex(@"(-)?[0-9.]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        Match mr = rrange.Match(value, 0, value.IndexOf(","));
                        if (mr.Success)
                        {
                            this.min = mr.Value;
                        }
                        //max
                        mr = rrange.Match(value, value.IndexOf(","), value.Length - value.IndexOf(","));
                        if (mr.Success)
                        {
                            this.max = mr.Value;
                        }
                    }
                }
                this.Valid = true;
                return input.Substring(0, matchIndex);
            }

            public DeltaXmlTextParser()
            {
                Min = "null";
                Max = "null";
                Unit = "null";
                Valid = false;
            }


            public bool Valid { get; private set; }

            string min = "null";
            string max = "null";
            string unit = "null";

            public string Min {
                get
                {
                    if (min.Equals("null") || min.Equals("")) return "null";
                    else return "'" + min + "'";
                }
                private set { min = value; }
            }
            public string Max
            {
                get
                {
                    if (max == "null" || max == "") return "null";
                    else return "'" + max + "'";
                }
                private set { max = value; }
            }
            public string Unit
            {
                get
                {
                    if (unit == "null" || unit == "") return "null";
                    else return "'" + unit + "'";
                }
                private set { unit = value; }
            }
        }

        
    }
}

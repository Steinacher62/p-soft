using ch.appl.psoft.db;
using ch.psoft.Util;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web;

namespace ch.appl.psoft.Wiki
{

    /// <summary>
    /// Konvertiert Wiki-Syntax in HTML nach folgenden Regeln:
    /// 
    /// Formatierung                        Wiki-Tag                    Ergebnis
    /// ------------------------------------------------------------------------
    /// kursiv                              ''Text''
    /// fett                                '''Text'''
    /// kursiv und fett                     '''''Text'''''
    /// unterstrichen                       __Text__
    /// Hyperlinks (absoluter Pfad)         http://www.PSOFT.ch           www.PSOFT.ch
    /// Hyperlinks (automatisch nummeriert) [http://www.PSOFT.ch]         [1]
    ///                                     [folder/page.htm]           [2]
    /// Hyperlinks (mit Text)               [http://www.PSOFT.ch B/D/H/]  B/D/H/
    /// Bilder (absoluter Pfad)             http://www.PSOFT.ch/a.gif
    /// Bilder (relavier Pfad)              [folder/a.gif]
    /// 
    /// E-Mail                              info@PSOFT.ch                 info@PSOFT.ch
    /// ISBN-Nummer                         ISBN 1-85249-113-2          ISBN 1-85249-113-2 (Link auf Buch bei Amazon)
    /// Monospace                            +-----------------------+  +-----------------------+
    ///   (Das erste Zeichen der Zeile       : Text mit Abständen    :  : Text mit Abständen    :
    ///     ist ein Leerschlag.)             +-----------------------+  +-----------------------+
    /// Rechtsbündig                        >> Text                                                              Text
    /// Zentriert                           >< Text                                             Text
    /// Blocksatz                           <> Text                     Text
    /// Einzug                              : 1. Ebene                  1. Ebene
    ///                                     :: 2. Ebene                     2. Ebene
    ///                                     ::: 3. Ebene                        3. Ebene
    /// Begriffe                            ; 1. Ebene:Definition       1. Ebene
    ///                                                                     Definition
    ///                                     ;; 2. Ebene:Definition          2. Ebene
    ///                                                                         Definition
    /// Liste                               * 1. Ebene                  * 1. Ebene
    ///                                     ** 2. Ebene                     o 2. Ebene
    ///                                     *** 3. Ebene                        § 3. Ebene
    /// Liste (nummeriert)                  # Ebene                     1. Ebene
    ///                                     ## Ebene                        1. Ebene
    ///                                     ### Ebene                           1. Ebene
    ///                                     # Ebene                     2. Ebene
    /// Linie                               ----                        (horizontale Linie)
    /// Überschriften                       = Titel1
    ///                                     == Titel2
    ///                                     === Titel3
    ///                                     ...
    ///                                     ======= Titel7
    /// Überschriften (automatisch num.)    = # Titel1                  1. Titel1
    ///                                     == # Titel2                 1.1 Titel2
    ///                                     === # Titel3                1.1.1 Titel3
    ///                                     == # Titel2                 1.2 Titel2
    ///                                     = # Titel1                  2. Titel1
    /// Tabellen
    /// Zelle (Start)                       !                           <td>
    /// Zelle über mehrer Spalten           !!                          <td colspan=2>
    ///                                     !!!                         <td colspan=3>
    /// Kein Zeilenumbruch                  !^                          <td nowrap>
    /// Rechtsbündig                        !>>                         <td align="right">
    /// Zentriert                           !><                         <td align="center">
    /// Blocksatz                           !<>                         <td align="justify">
    /// Kopfzeile                           !+                          <th ...>
    /// 
    /// 
    /// ------------------------
    /// 
    /// Bilder (raufgeladen zu owner-UID)   [[Bild:b.gif|Formatierung|Positionierung|Beschreibung]]
    ///       mögliche Formatierungen:
    ///             - thumb (wird als Miniaturbild dargestellt)
    ///       mögliche Positionierungen:
    ///             - left  (wird am linken Rand positioniert)
    ///             - right (wird am rechten Rand positioniert)
    ///             
    /// Hyperlink auf Wissenseintrag        [[Eintrag|Titel für Darstellung]]
    /// 
    /// Hyperlink auf p-soft-Objekt          [[ObjektTyp:Titel des Objekts|Titel für Darstellung]]
    ///      als ObjektTyp können folgende Objekte verwendet werden:
    ///           - UID (Objekt wird durch seine UID identifiziert)
    ///           - Person
    ///           - Kontakt / contact
    ///           - Projekt / project
    ///           - Pendenzenliste / tasklist
    ///           - Dokument / document
    /// </summary>
    public class Wiki2HTML{
        private const char NEW_LINE = '\n';
        private Regex _rexLine;
        private Regex _rexItalic;
        private Regex _rexBold;
        private Regex _rexUnderline;
        private Regex _rexWikiReference;
        private Regex _rexLinks;
        private int _linkNr;
        private string _stack;

        private const string CLASS_WIKI_EXTERNAL = "wikiExternal";
        private const string CLASS_WIKI_NEW      = "wikiNew";
        private const string CLASS_WIKI_INVALID  = "wikiInvalid";

        public Wiki2HTML(){
            _rexLine      = new Regex("^(<<|><|>>|<>|!|:{1,}|\\*{1,}|#{1,}|;{1,}|\\-{4,}|={1,6})?(.*)");
            _rexItalic    = new Regex("''([^']*)(''|$)");
            _rexBold      = new Regex("'''([^']*)('''|$)");
            _rexUnderline = new Regex("__([^_]*)(__|$)");
            string URLChars    = "a-z0-9;/\\?:@&=\\+\\$,\\-_\\.!~\\*'\\(\\)#%\\\\"; // a-zA-Z0-9;/?:@&=+$,-_.!~*'()#%\
            string URLEndChars = "a-z0-9;/\\?:@&=\\+\\$\\-_\\\\";                   // a-zA-Z0-9;/?:@&=+$-_\
            _rexLinks = new Regex("(http[s]?://([" + URLChars + "]*[" + URLEndChars + "]))|" + 
                                  "\\[([" + URLChars + "]*[" + URLEndChars + "])\\]|" +
                                  "\\[([" + URLChars + "]*[" + URLEndChars + "])\\s*([^\\]]*)\\]|" +
                                  "([a-z0-9\\._]+@[a-z0-9\\._]+[a-z0-9_]+)|" +
                                  "(ISBN[:]? ([0-9A-Z\\-]+))|" + 
                                  "(www\\.([" + URLChars + "]*[" + URLEndChars + "]))", RegexOptions.IgnoreCase);
            string wikiChars = "^\\]^\\|";
            _rexWikiReference = new Regex("\\[\\[(([a-z]*):)?([" + wikiChars + "]+)(\\|([" + wikiChars + "]*))*\\]\\]", RegexOptions.IgnoreCase);
        }

        private void Init(){
            _linkNr = 1;
            _stack = "";
        }

        private string MultiString(int n, string s){
            string retValue = "";
            for (int i=0; i<n; i++){
                retValue += s;
            }
            return retValue;
        }

        private string OpenBlock(char sBlock, int n){
            string retValue = "";
            string tag = "";
            switch (sBlock){
                case 'B':
                    tag = "<blockquote>";
                    break;

                case 'O':
                    tag = "<ol>";
                    break;

                case 'U':
                    tag = "<ul>";
                    break;
            
                case 'D':
                    tag = "<dl>";
                    break;
            
                case 'T':
                    tag = "<table>";
                    break;
            }
            retValue = MultiString(n, tag + NEW_LINE);
            for (int i=0; i<n; i++){
                _stack += sBlock;
            }
            return retValue;
        }

        private string CloseBlock(int n){
            string retValue = "";
            for (int i=0; i<n; i++){
                string tag = "";
                switch (_stack[_stack.Length-1]){
                    case 'B':
                        tag = "</blockquote>";
                        break;

                    case 'O':
                        tag = "</ol>";
                        break;

                    case 'U':
                        tag = "</ul>";
                        break;
            
                    case 'D':
                        tag = "</dl>";
                        break;
            
                    case 'T':
                        tag = "</table>";
                        break;
                }
                retValue += tag + NEW_LINE;
                _stack = _stack.Substring(0, _stack.Length-1);
            }
            return retValue;
        }

        private string CloseBlocks(){
            return CloseBlock(_stack.Length);
        }

        private string StartBlock(int iDepth, char sBlock){
            string retValue = "";
            
            if (iDepth == _stack.Length){
                if (_stack[_stack.Length-1] != sBlock){
                    retValue = CloseBlock(1) + OpenBlock(sBlock, 1);
                }
            }
            else if (iDepth < _stack.Length){
                retValue = CloseBlock(_stack.Length - iDepth);
                if (_stack[_stack.Length-1] != sBlock){
                    retValue = CloseBlock(1) + OpenBlock(sBlock, 1);
                }
            }
            else if (iDepth > _stack.Length){
                retValue = OpenBlock(sBlock, iDepth-_stack.Length);
            }

            return retValue;
        }

        private string Trim(string text){
            return text.Trim();
        }

        private string Left(string text, int n){
            string retValue = "";
            if (text.Length > 0){
                int nrOfChars = Math.Min(text.Length, n);
                retValue = text.Substring(0, nrOfChars);
            }
            return retValue;
        }

        private string Right(string text, int n){
            string retValue = "";
            if (text.Length > 0){
                int startPos = Math.Max(text.Length - n, 0);
                int nrOfChars = Math.Min(text.Length, n);
                retValue = text.Substring(startPos, nrOfChars);
            }
            return retValue;
        }

        private string ReplaceFirst(string text, string oldValue, string newValue){
            string retValue = text;
            int pos = text.IndexOf(oldValue);
            if (pos >= 0){
                retValue = text.Remove(pos, oldValue.Length);
                retValue = retValue.Insert(pos, newValue);
            }
            return retValue;
        }

        private string Reference(string sURL, string sText, string toolTip, string sTarget, string cssClass){
            string retValue = "";
            string strExt = sURL.Substring(sURL.LastIndexOf(".")+1).ToLower();
            if (strExt == "gif" || strExt == "jpg" || strExt == "jpeg" || strExt == "png"){
                if (sText == ""){
                    sText = sURL;
                }
                retValue = "<img src='" + sURL + "' alt=\"" + HttpUtility.HtmlEncode(toolTip) + "\">";
            }
            else{
                if (sText == ""){
                    sText = "[" + _linkNr++ + "]";
                }
                retValue = "<a href='" + sURL + "' title=\"" + HttpUtility.HtmlEncode(toolTip) + "\"";
                if (sTarget != ""){
                    retValue += " target='" + sTarget + "'";
                }
                if (cssClass != ""){
                    retValue += " class='" + cssClass + "'";
                }
                retValue += ">" + Trim(sText) + "</a>";
            }
            return retValue;
        }

        private string WikiReference(string referenceName, string reference, string[] parameters, DBData db, long ownerUID){
            string retValue = "";
            switch (referenceName.ToLower()){
                case "bild":
                case "image":
                    string positionStyle = "wikiFloatRight";
                    string thumbStyle = "";
                    string imgStyle = "wiki";
                    string description = "";
                    string imageURL = "";
                    int width = 0;
                    for (int i=0; i<parameters.Length; i++){
                        switch(parameters[i].ToLower()){
                            case "left":
                                positionStyle = "wikiFloatLeft";
                                break;
                
                            case "right":
                                positionStyle = "wikiFloatRight";
                                break;

                            case "thumb":
                                imageURL = db.WikiImage.getThumbURL(ownerUID, reference);
                                width = db.WikiImage.getThumbWidth(ownerUID, reference);
                                thumbStyle = imgStyle = "wikiThumb";
                                break;

                            default:
                                if (parameters[i] != ""){
                                    description = parameters[i];
                                }
                                break;
                        }
                    }
                    string target = "_blank";
                    string imageLink = db.WikiImage.getImageURL(ownerUID, reference);
                    if (imageLink == ""){
                        target = "_self";
                        imageLink = psoft.Wiki.ImageAdd.GetURL("ownerUID",ownerUID, "imageTitle",reference);
                    }
                    if (imageURL == ""){
                        imageURL = db.WikiImage.getImageURL(ownerUID, reference);
                    }
                    if (description == ""){
                        description = ch.psoft.Util.Validate.GetValid(db.WikiImage.getDescription(ownerUID, reference), reference);
                    }
                    retValue = "<div class='" + positionStyle + "'>";
                    if (thumbStyle != ""){
                        retValue +=  "<div class='" + thumbStyle + "'>";
                    }
                    retValue += "<a href='" + imageLink + "' target='" + target + "'>" +
                        "<img src='" + imageURL + "' alt=\"" + HttpUtility.HtmlEncode(description) + "\" class='" + imgStyle + "'>" + 
                        "</a>";
                    if (thumbStyle != ""){
                        if (width <= 0){
                            width = Interface.DBObjects.WikiImage._MAX_THUMB_WIDTH;
                        }
                        retValue += "<div class='wikiThumbCaption' style='width:" + width + "px;'>" + HttpUtility.HtmlEncode(description) + "</div></div>";
                    }
                    retValue += "</div>";
                    break;

                default:
                    string referenceTitle = "";
                    for (int i=0; i<parameters.Length; i++){
                        switch(parameters[i]){
                            case "TODO:xxx":
                                break;

                            default:
                                if (parameters[i] != ""){
                                    referenceTitle = parameters[i];
                                }
                                break;
                        }
                    }
                    if (referenceTitle == ""){
                        referenceTitle = reference;
                    }

                    long UID = -1L;
                    switch (referenceName.ToLower()){
                        case "uid":
                            UID = ch.psoft.Util.Validate.GetValid(reference, -1L);
                            reference = db.UID2NiceName(UID);
                            if (referenceTitle == UID.ToString()){
                                referenceTitle = reference;
                            }
                            break;

                        case "person":
                            UID = db.NiceName2UID(reference, "PERSON");
                            break;

                        case "contact":
                        case "kontakt":
                            UID = db.NiceName2UID(reference, "FIRM");
                            if (UID <= 0){
                                UID = db.NiceName2UID(reference, "PERSON");
                            }
                            break;

                        case "project":
                        case "projekt":
                            UID = db.Project.getUIDByTitle(reference);
                            break;

                        case "tasklist":
                        case "pendenzenliste":
                            if (Global.isModuleEnabled("tasklist")){
                                UID = db.Tasklist.getUIDByTitle(reference);
                            }
                            break;

                        case "document":
                        case "dokument":
                            UID = db.NiceName2UID(reference, "DOCUMENT");
                            break;

                        case "": // link auf Wissenselement
                            if (Global.isModuleEnabled("knowledge")){
                                UID = db.Knowledge.getUIDByTitle(reference);
                                if (UID <= 0){
                                    retValue = Reference(psoft.Knowledge.EditKnowledge.GetURL("mode","add", "title",reference), referenceTitle, reference, "", CLASS_WIKI_NEW);
                                }
                            }
                            break;
                    }
                    if (retValue == ""){
                        if (UID > 0 && db.hasUIDAuthorisation(DBData.AUTHORISATION.READ, UID, DBData.APPLICATION_RIGHT.COMMON, true, true)){

							if(db.UID2Tablename(UID) == "DOCUMENT")
							{
							   long documentId = db.UID2ID(UID);
                               retValue = Reference(psoft.Document.GetDocument.GetURL("documentID",documentId), referenceTitle, reference, "_blank", "");	
							}
							else
						    {
								retValue = Reference(psoft.Goto.GetURL("UID",UID), referenceTitle, reference, "", "");													
							}
							
                        }
                        else{
                            retValue = Reference(psoft.NotFound.GetURL(), referenceTitle, reference, "", CLASS_WIKI_INVALID);
                        }
                    }
                    break;
            }
            return retValue;
        }

        private string TableCell(string sContent, int iColSpan, int iRowSpan, string sHAlign, string sVAlign, bool bNoWrap, bool bHeader){
            string retValue = "";
            if (bHeader){
                retValue = "<th";
            }
            else{
                retValue = "<td";
            }
            if (iColSpan > 1){
                retValue += " colspan=" + iColSpan;
            }
            if (iRowSpan > 1){
                retValue += " rowspan=" + iRowSpan;
            }
            if (sHAlign != ""){
                retValue += " align=" + sHAlign;
            }
            if (sVAlign != ""){
                retValue += " valign=" + sVAlign;
            }
            if (bNoWrap){
                retValue += " nowrap";
            }
            retValue += ">" + Trim(sContent) + "</td>" + NEW_LINE;

            return retValue;
        }

        public string Translate(string sSource, DBData db, long ownerUID, ref AutoNumbering autoNumbering, int indentLevel, ref ArrayList contentsEntries){
            string retValue = "";

            if (sSource != ""){
                Init();
                contentsEntries = new ArrayList();
                string[] arrLines = sSource.Split(NEW_LINE);
                string s = "";
                string ss = "";
                foreach (string arrLine in arrLines){
                    s += arrLine;
                    if (s == ""){
                        //*** add an empty line
                        retValue += CloseBlocks() + "<br />" + NEW_LINE;
                    }
                    else if (s[s.Length-1] == '\\'){
                        //*** remove \ and add the next line
                        s = s.Substring(0, s.Length-1);
                    }
                    else if (s[0] == ' '){
                        //*** preformatted text
                        retValue += CloseBlocks() + "<code>" + ReplaceFirst(s, " ", "&nbsp;") + "</code><br />" + NEW_LINE;
                        s = "";
                    }
                    else{
                        // parse the line
                        bool blnAddBR = true;

                        //*** process text formatting
                        s = _rexBold.Replace(s,"<b>$1</b>");
                        s = _rexItalic.Replace(s,"<i>$1</i>");
                        s = _rexUnderline.Replace(s,"<u>$1</u>");

                        //*** process wiki references
                        MatchCollection matches = _rexWikiReference.Matches(s);
                        foreach (Match match in matches){
                            string[] parameters = new string[match.Groups[5].Captures.Count];
                            for (int i=0; i<match.Groups[5].Captures.Count; i++){
                                parameters[i] = match.Groups[5].Captures[i].ToString();
                            }
                            s = ReplaceFirst(s, match.ToString(), WikiReference(match.Groups[2].ToString(), match.Groups[3].ToString(), parameters, db, ownerUID));
                        }

                        //*** process links and images
                        matches = _rexLinks.Matches(s);
                        foreach (Match match in matches){
                            if (match.Groups[1].ToString() != ""){
                                s = ReplaceFirst(s, match.ToString(), Reference(match.Groups[1].ToString(), match.Groups[2].ToString(), match.Groups[2].ToString(), "_blank", CLASS_WIKI_EXTERNAL));
                            }
                            else if (match.Groups[3].ToString() != ""){
                                s = ReplaceFirst(s, match.ToString(), Reference(match.Groups[3].ToString(), "", match.Groups[3].ToString(), "_blank", CLASS_WIKI_EXTERNAL));
                                _linkNr++;
                            }
                            else if (match.Groups[4].ToString() != ""){
                                s = ReplaceFirst(s, match.ToString(), Reference(match.Groups[4].ToString(), match.Groups[5].ToString(), match.Groups[5].ToString(), "_blank", CLASS_WIKI_EXTERNAL));
                            }
                            else if (match.Groups[6].ToString() != ""){
                                s = ReplaceFirst(s, match.ToString(), "<a href=mailto:" + match.Groups[6].ToString() + ">" + match.Groups[6].ToString() + "</a>");
                            }
                            else if (match.Groups[7].ToString() != ""){
                                ss = ReplaceFirst(match.Groups[8].ToString(), "-", "");
                                s = ReplaceFirst(s, match.ToString(), Reference("http://www.amazon.com/exec/obidos/ASIN/" + ss, match.Groups[7].ToString(), match.Groups[7].ToString(), "_blank", CLASS_WIKI_EXTERNAL));
                            }
                            else if (match.Groups[9].ToString() != ""){
                                s = ReplaceFirst(s, match.ToString(), Reference("http://" + match.Groups[9].ToString(), match.Groups[9].ToString(), match.Groups[9].ToString(), "_blank", CLASS_WIKI_EXTERNAL));
                            }
                        }

                        //*** process line and block formatting
                        matches = _rexLine.Matches(s);
                        foreach (Match match in matches){
                            int intDepth = match.Groups[1].ToString().Length;
                            if (match.Groups[1].ToString() == ""){
                                s = CloseBlocks() + match.Groups[2].ToString().Trim();
                            }
                            else if (Left(match.Groups[1].ToString(),2) == "<<"){
                                ss = "<div align='left'>" + Trim(match.Groups[2].ToString()) + "</div>";
                                s = ReplaceFirst(s, match.ToString(), ss);
                                blnAddBR = false;
                            }
                            else if (Left(match.Groups[1].ToString(),2) == "><"){
                                ss = "<div align='center'>" + Trim(match.Groups[2].ToString()) + "</div>";
                                s = ReplaceFirst(s, match.ToString(), ss);
                                blnAddBR = false;
                            }
                            else if (Left(match.Groups[1].ToString(),2) == ">>"){
                                ss = "<div align='right'>" + Trim(match.Groups[2].ToString()) + "</div>";
                                s = ReplaceFirst(s, match.ToString(), ss);
                                blnAddBR = false;
                            }
                            else if (Left(match.Groups[1].ToString(),2) == "<>"){
                                ss = "<div align='justify'>" + Trim(match.Groups[2].ToString()) + "</div>";
                                s = ReplaceFirst(s, match.ToString(), ss);
                                blnAddBR = false;
                            }    
                            else if (Left(match.Groups[1].ToString(),1) == "!"){// table
                                ss = StartBlock(1, 'T') + "  <tr>" + NEW_LINE;
                                string[] arr = Trim(match.Groups[2].ToString()).Split('!');
                                int intColSpan = 1;
                                int intRowSpan = 1;
                                for (int i=0; i<arr.Length; i++){
                                    if (Trim(arr[i]) == "" ){
                                        intColSpan++;
                                    }
                                    else{
                                        string strHAlign = "";
                                        string strVAlign = "top";
                                        bool blnNoWrap = false;
                                        bool blnHeader = false;
                                        if (Left(arr[i],1) == "+" ){
                                            blnHeader = true;
                                            arr[i] = Trim(Right(arr[i], arr[i].Length - 1));
                                        }
                                        if (Left(arr[i],2) == "<<" ){
                                            strHAlign = "left";
                                            arr[i] = Trim(Right(arr[i], arr[i].Length - 2));
                                        }
                                        if (Left(arr[i],2) == ">>" ){
                                            strHAlign = "right";
                                            arr[i] = Trim(Right(arr[i], arr[i].Length - 2));
                                        }
                                        if (Left(arr[i],2) == "><" ){
                                            strHAlign = "center";
                                            arr[i] = Trim(Right(arr[i], arr[i].Length - 2));
                                        }
                                        if (Left(arr[i],2) == "<>" ){
                                            strHAlign = "justify";
                                            arr[i] = Trim(Right(arr[i], arr[i].Length - 2));
                                        }
                                        if (Left(arr[i],1) == "^" ){
                                            blnNoWrap = true;
                                            arr[i] = Trim(Right(arr[i], arr[i].Length - 1));
                                        }
                                        ss = ss + TableCell(arr[i], intColSpan, intRowSpan, strHAlign, strVAlign, blnNoWrap, blnHeader);
                                        intColSpan = 1;
                                        intRowSpan = 1;
                                    }
                                }
                                ss += "  </tr>";
                                s = ReplaceFirst(s, match.ToString(), ss);
                                blnAddBR = false;
                            }
                            else if (Left(match.Groups[1].ToString(),1) == ":"){
                                if (Trim(match.Groups[2].ToString()) == ""){
                                    ss = "<br />" + NEW_LINE + "<br />" + NEW_LINE;
                                }
                                else{
                                    ss = StartBlock(intDepth, 'B') + Trim(match.Groups[2].ToString());
                                }
                                s = ReplaceFirst(s, match.ToString(), ss);
                                blnAddBR = false;
                            }
                            else if (Left(match.Groups[1].ToString(),1) == "*"){
                                ss = StartBlock(intDepth, 'U') + "<li>" + Trim(match.Groups[2].ToString()) + "</li>";
                                s = ReplaceFirst(s, match.ToString(), ss);
                                blnAddBR = false;
                            }
                            else if (Left(match.Groups[1].ToString(),1) == "#"){
                                ss = StartBlock(intDepth, 'O') + "<li>" + Trim(match.Groups[2].ToString()) + "</li>";
                                s = ReplaceFirst(s, match.ToString(), ss);
                                blnAddBR = false;
                            }
                            else if (Left(match.Groups[1].ToString(),1) == ";" ){
                                ss = StartBlock(intDepth, 'D');
                                string[] arr = Trim(match.Groups[2].ToString()).Split(':');
                                if (arr.Length > 0){
                                    ss += "<dt>" + Trim(arr[0]) + "</dt>";
                                }
                                if (arr.Length > 1){
                                    ss += "<dd>" + Trim(arr[1]) + "</dd>";
                                }
                                s = ReplaceFirst(s, match.ToString(), ss);
                                blnAddBR = false;
                            }
                            else if (Left(match.Groups[1].ToString(),1) == "-" ){
                                ss = CloseBlocks() + "<hr>";
                                s = ReplaceFirst(s, match.ToString(), ss);
                                blnAddBR = false;
                            }
                            else if (Left(match.Groups[1].ToString(),1) == "=" ){
                                int hn = intDepth + indentLevel;
                                string autoNumber = autoNumbering.GetNextNumber(hn - 1);
                                string anchor = autoNumber.Replace(".", "_");
                                ss = CloseBlocks() + "<a name='" + anchor + "'></a><h" + hn + ">";
                                string sss = Trim(match.Groups[2].ToString());
                                if (Left(sss,1) == "#" ){
                                    ss += autoNumber + " ";
                                    sss = Trim(Right(sss, sss.Length - 1));
                                }
                                ss += sss + "</h" + hn + ">";
                                s = ReplaceFirst(s, match.ToString(), ss);
                                contentsEntries.Add(new ContentsEntry(anchor, autoNumber, sss));
                                blnAddBR = false;
                            }
                        }

                        retValue += s;
                        if (blnAddBR){
                            retValue += "<br />";
                        }
                        retValue += NEW_LINE;
                        s = "";
                    }
                }
                retValue += CloseBlocks();
            }


            return retValue;
        }
    }

    public class ContentsEntry{
        public string _anchor;
        public string _autoNumber;
        public string _title;

        public ContentsEntry(string anchor, string autoNumber, string title){
            _anchor = anchor;
            _autoNumber = autoNumber;
            _title = title;
        }
    }
}
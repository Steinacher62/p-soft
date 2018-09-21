/** lexical analyzer / scanner File Specification */ 

/*****************************************************************
 *  User Code Section
 */

namespace ch.appl.psoft.Morph.lexer.generated {

using System;
using System.IO;

using ch.appl.psoft.Morph.lexer;



%% 
/*****************************************************************
 * Options and Declarations Section 
 */

/* Name the generated class Lexer */
%class Lexer

%line
%column

/* Must be unicode */
%unicode


/********************************************************************************
 *  2. Declarations 
 */


%{ 
  private Yytoken symbol(int type) { 
    return new Yytoken(type, yyline, yycolumn); 
  
  } 
  private Yytoken symbol(int type, Object value) { 
    return  new Yytoken(type, yyline, yycolumn, value); 
    
  } 
   
  public bool isZzAtEOF() {
	return this.zzAtEOF;
  }

///attributes
    string stringbufHeading;
	HeadingManager 	headingManager;
	
    int headingLen = 0;
   
%} 



/*******************************************************************************
 *  3. Macro Declarations 
 */ 

LineTerminator = \r | \n | \r\n 

url1 = [a-zA-Z]{3}"."[a-zA-Z0-9\-._]+"."[a-zA-Z0-9]{2,3}[a-zA-Z0-9._/]*
url2 = ([0-9]{1,3}"."){3}[0-9]{1,3}[a-zA-Z0-9._/]*

url = "http://"{url1}|{url1}|"http://"{url2}


%xstate YYHEADING
 


%% 
/*****************************************************************
 * Lexical Rules Section
 */


/*   YYINITIAL is the state at which the lexer begins scanning.  */ 
<YYINITIAL> { 


^("#")+                     { return symbol(sym.LISTNUMB, yytext().Length ); } 
^("*")+                     { return symbol(sym.LISTUNNUMB, yytext().Length ); } 
"''"	                    { return symbol(sym.ITALIC); }
"'''"          	            { return symbol(sym.BOLD);  }
"'''''"         	        { return symbol(sym.BOLDITALIC); }
"__"                        { return symbol(sym.UNDERLINED); }
^("----")[^\n\r-]*{LineTerminator}       { return symbol(sym.HLINE); }


"[[Bild:"[^'\]\]]+"]]"     { return symbol( sym.IMAGE, yytext() ); }

"[[UID:"[:digit:]+"]]"     { return symbol( sym.INTERNAL_LINK, yytext() ); }
"[[UID:"[:digit:]+"|"[:digit:]+"]]"     { return symbol( sym.INTERNAL_LINK, yytext() ); }


^("=")+((" #"){0,1}|(" "){0,1})   { headingManager = new HeadingManager(yytext()); yybegin(YYHEADING); }

^(":")+                    { return symbol(sym.LISTSIMPLE, yytext().Length );}

"\\"{LineTerminator}       { /* skip */ }

{url}                       {  return symbol(sym.SIMPLEURL, yytext() ); }

"["{url}" "[^\]\n\r\t ]+"]"  {  return symbol(sym.URL, yytext()); }

[[:letter:][:digit:]_.]+"@"[[:letter:][:digit:]_.]+"."[:letter:]{2,3}    { return symbol(sym.EMAIL, yytext() ); }

{LineTerminator}            { return symbol(sym.EOL, yytext()); } 

">>"						{ return symbol(sym.PARAGRAPH_RIGHT ); }
"><"						{ return symbol(sym.PARAGRAPH_CENTER ); }
"<>"						{ return symbol(sym.PARAGRAPH_BLOCK ); }


} 




<YYHEADING> {
/* special format directly handled here */

{LineTerminator}            { headingManager.flush(); yybegin(YYINITIAL); 
                              return symbol( sym.HEADING, headingManager ); } 
                              
"__"                        { headingManager.setUnderline(); }
"''"	                    { headingManager.setItalic(); }
"'''"          	            { headingManager.setBold();  }
"'''''"         	        { headingManager.setBoldItalic(); }


[^\n\r]                    { Console.WriteLine("H<"+yytext()+">"); headingManager.append(yytext()); }

}



.                       { /*Console.WriteLine("Any character <"+yytext()+">");*/
                          return symbol(sym.CHAR, yytext() );
                        } 


%%
}
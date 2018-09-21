namespace ch.appl.psoft.Morph.lexer
{
    public class Yytoken
    {
        // Fields
        private int column;
        private int line;
        private int symbolId;
        private object value;

        // Methods
        public Yytoken(int symbolId, int line, int column)
        {
            this.symbolId = symbolId;
            this.line = line;
            this.column = column;
        }

        public Yytoken(int symbolId, int line, int column, object value)
        {
            this.symbolId = symbolId;
            this.line = line;
            this.column = column;
            this.value = value;
        }

        public override string ToString()
        {
            return string.Concat(new object[] { "Sym: ", this.symbolId, " Line: ", this.line, " Column: ", this.column, " Value: ", this.value });
        }

        // Properties
        public int SymbolId
        {
            get
            {
                return this.symbolId;
            }
        }

        public object Value
        {
            get
            {
                return this.value;
            }
        }
    }
}

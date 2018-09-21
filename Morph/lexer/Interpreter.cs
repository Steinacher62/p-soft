using ch.appl.psoft.db;
using ch.psoft.Util;
using System;
using System.Text.RegularExpressions;
namespace ch.appl.psoft.Morph.lexer
{
    public abstract class Interpreter
    {
        // Methods
        protected Interpreter()
        {
        }

        // Nested Types
        public class Image : Interpreter.TokenInterpreter
        {
            // Fields
            private string[] attributes;
            private string description;
            private string path;
            private int width;

            // Methods
            public Image(string text, int begl, int endl, DBData db, long owner_uid)
                : base(text, begl, endl)
            {
                string title = this.parseInfos();
                this.path = "http://localhost" + db.WikiImage.getThumbURL(owner_uid, title);
                this.description = db.WikiImage.getDescription(owner_uid, title);
                this.width = db.WikiImage.getThumbWidth(owner_uid, title);
            }

            private bool check(string attr)
            {
                if (this.attributes != null)
                {
                    foreach (string str in this.attributes)
                    {
                        if (str == attr)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            public bool isLeft()
            {
                return this.check("left");
            }

            public bool isRight()
            {
                return this.check("right");
            }

            public bool isThumb()
            {
                return this.check("thumb");
            }

            private string parseInfos()
            {
                if (base.depuredContent.IndexOf("|") < 0)
                {
                    return base.depuredContent;
                }
                int index = base.depuredContent.IndexOf("|");
                string str = base.depuredContent.Substring(0, index);
                int length = base.depuredContent.Length;
                string input = base.depuredContent.Substring(index + 1, (length - index) - 1).ToLower();
                this.attributes = Regex.Split(input, @"\|");
                return str;
            }

            // Properties
            public string Description
            {
                get
                {
                    return this.description;
                }
            }

            public string Path
            {
                get
                {
                    return this.path;
                }
            }
        }

        public class InternalLink : Interpreter.TokenInterpreter
        {
            // Fields
            private string niceName;

            // Methods
            public InternalLink(string text, int begl, int endl, DBData db, long owner_uid)
                : base(text, begl, endl)
            {
                try
                {
                    int num = int.Parse(base.depuredContent);
                    this.niceName = db.UID2NiceName((long)num);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception, Logger.ERROR);
                }
            }

            // Properties
            public string NiceName
            {
                get
                {
                    return this.niceName;
                }
            }
        }

        public class ListToken
        {
            // Fields
            private int level;

            // Methods
            public ListToken(string token)
            {
                this.level = token.Length;
            }

            public override string ToString()
            {
                return "<ListToken: Symbol Only>";
            }

            // Properties
            public int Level
            {
                get
                {
                    return this.level;
                }
            }
        }

        public class TokenInterpreter
        {
            // Fields
            protected string depuredContent;

            // Methods
            protected TokenInterpreter(string text, int begl, int endl)
            {
                int length = text.Length - (begl + endl);
                this.depuredContent = text.Substring(begl, length);
            }

            public override string ToString()
            {
                return this.depuredContent;
            }
        }

        public class UrlInterpreter : Interpreter.TokenInterpreter
        {
            // Fields
            private string description;
            private string url;

            // Methods
            public UrlInterpreter(string text, int begl, int endl)
                : base(text, begl, endl)
            {
                int index = base.depuredContent.IndexOf(" ");
                this.url = base.depuredContent.Substring(0, index);
                this.description = base.depuredContent.Substring(index + 1, (base.depuredContent.Length - index) - 1);
            }

            // Properties
            public string Description
            {
                get
                {
                    return this.description;
                }
            }

            public string Url
            {
                get
                {
                    return this.url;
                }
            }
        }
    }
}
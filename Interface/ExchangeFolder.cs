using System.Collections;

namespace ch.appl.psoft.Interface
{
    /// <summary>
    /// Summary description for ExchangeFolder.
    /// </summary>
    public class ExchangeFolder : ExchangeItem
	{

		public ExchangeFolder(long exchangeid, string exchangetitle, string title, string href, string ishidden, string isfolder, string username, string password, string host, string domain, string creationdate) 
			: base(exchangeid, exchangetitle, title, href, ishidden, isfolder, username, password, host, domain, creationdate)
		{	
		}

		public ExchangeFolder(long exchangeid, string exchangetitle, string title, string href, bool ishidden, bool isfolder, string username, string password, string host, string domain, string creationdate) 
			: base(exchangeid, exchangetitle, title, href, ishidden, isfolder, username, password, host, domain, creationdate)
		{	
		}

		public ExchangeFolder(ExchangeItem ei) : this(ei.Exchangeid, ei.ExchangeTitle, ei.Title, ei.Href, ei.Ishidden, ei.Isfolder, ei.Username, ei.Password, ei.Host, ei.Domain, ei.Creationdate)
		{
		}

		public ArrayList getMessages() 
		{
			ExchangeHelper eh = new ExchangeHelper(this.Exchangeid, this.ExchangeTitle, this.Username, this.Password, this.Href);
			ArrayList retVal = eh.getMessages();
			return retVal;
		}

		public override string ToString() 
		{
			return "Folder: " + this.Title + " (" + this.Username + "@" + this.Host + "." + this.Domain + ")";
		}
	}
}

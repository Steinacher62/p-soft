using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace ch.appl.psoft.Interface
{
    /// <summary>
    /// Summary description for ExchangeMessage.
    /// </summary>
    public class ExchangeMessage : ExchangeItem
	{

		private static Regex regex = new Regex("(?:(?:(?:(.*)(?:[-]{1}\\d*\\.EML))|(?:(.*)(?:\\.EML))))");
		public static string TEXTDESCRIPTION = "e:textdescription";
		public static string HTMLDESCRIPTION = "e:htmldescription";
		public static string FROM = "e:from";
		public static string HASATTACHEMENTS = "e:hasattachment";
		public static string NROFATTACHEMENTS = "e:nrofattachements";

		#region Properties
		
		public string HumanReadableTitle
		{
			get 
			{
				Match m = ExchangeMessage.regex.Match(this.Title);
				if(m.Success) 
				{
					String s = m.Groups[1].ToString() == "" ? m.Groups[2].ToString() : m.Groups[1].ToString();
					return s;
				} 
				else 
				{
					return this.Title;
				}
			}
		}

		#endregion

		public ExchangeMessage(long exchangeid, string exchangetitle, string title, string href, string ishidden, string isfolder, string username, string password, string host, string domain, string creationdate) 
			: base(exchangeid, exchangetitle, title, href, ishidden, isfolder, username, password, host, domain, creationdate)
		{	
		}

		public ExchangeMessage(long exchangeid, string exchangetitle, string title, string href, bool ishidden, bool isfolder, string username, string password, string host, string domain, string creationdate) 
			: base(exchangeid, exchangetitle, title, href, ishidden, isfolder, username, password, host, domain, creationdate)
		{	
		}

		public ExchangeMessage(ExchangeItem ei) : this(ei.Exchangeid, ei.ExchangeTitle, ei.Title, ei.Href, ei.Ishidden, ei.Isfolder, ei.Username, ei.Password, ei.Host, ei.Domain, ei.Creationdate)
		{
		}

		public ArrayList getAttachements() 
		{
			ExchangeHelper eh = new ExchangeHelper(this.Exchangeid, this.ExchangeTitle, this.Username, this.Password, this.Href);
			return eh.getAttachements(this.Href);
		}

		public override string ToString() 
		{
			return "Message: " + this.HumanReadableTitle;
		}
	}
}

using System.Collections;

namespace ch.appl.psoft.Interface
{
    /// <summary>
    /// Summary description for ExchangeItem.
    /// </summary>
    public class ExchangeItem
	{
		private long exchangeid;
		private string exchangetitle;
		private string title;
		private string href;
		private string username;
		private string password;
		private string host;
		private string domain;
		private bool ishidden;
		private bool isfolder;
		private string creationdate;
		private Hashtable attributes = new Hashtable();


		#region Properties

		public string Title
		{
			get {return this.title;}
			set {this.title = value;}
		}

		public string Href
		{
			get {return this.href;}
			set {this.href = value;}
		}

		public bool Ishidden
		{
			get {return this.ishidden;}
			set {this.ishidden = value;}
		}

		public bool Isfolder
		{
			get {return this.isfolder;}
			set {this.isfolder = value;}
		}

		public string ExchangeTitle
		{
			get {return this.exchangetitle;}
			set {this.exchangetitle = value;}
		}

		public string Username
		{
			get {return this.username;}
			set {this.username = value;}
		}

		public string Password
		{
			get {return this.password;}
			set {this.password = value;}
		}

		public string Host
		{
			get {return this.host;}
			set {this.host = value;}
		}

		public string Domain
		{
			get {return this.domain;}
			set {this.domain = value;}
		}

		public long Exchangeid
		{
			get {return this.exchangeid;}
			set {this.exchangeid = value;}
		}

		public string Creationdate
		{
			get {return this.creationdate;}
			set {this.creationdate = value;}
		}

		#endregion

		/*	
		public ExchangeItem(string title, string href, string ishidden, string isfolder) 
		{
			this.title = title;
			this.href = href;
			this.Ishidden = (ishidden == "1");
			this.Isfolder = (isfolder == "1");
		}
		*/

		public ExchangeItem(long exchangeid, string exchangetitle, string title, string href, string ishidden, string isfolder, string username, string password, string host, string domain, string creationdate) 
		{
			this.exchangeid = exchangeid;
			this.exchangetitle = exchangetitle;
			this.title = title;
			this.href = href;
			this.Ishidden = (ishidden == "1");
			this.Isfolder = (isfolder == "1");
			this.username = username;
			this.password = password;
			this.host = host;
			this.domain = domain;
			this.creationdate = creationdate;
		}

		public ExchangeItem(long exchangeid, string exchangetitle, string title, string href, bool ishidden, bool isfolder, string username, string password, string host, string domain, string creationdate) 
		{
			this.exchangeid = exchangeid;
			this.exchangetitle = exchangetitle;
			this.title = title;
			this.href = href;
			this.Ishidden = ishidden;
			this.Isfolder = isfolder;
			this.username = username;
			this.password = password;
			this.host = host;
			this.domain = domain;
			this.creationdate = creationdate;
		}

		public void setAdditionalAttribute(string key, string val) 
		{
			attributes.Add(key,val);
		}

		public string getAdditionalAttribute(string key) 
		{
			return (string) attributes[key];
		}

		public override string ToString() 
		{
			return this.title + " (" + this.username + "@" + this.host + "." + this.domain + ")";
		}
	}
	
}

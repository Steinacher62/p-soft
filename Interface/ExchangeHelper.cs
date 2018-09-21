using ch.appl.psoft.db;
using ch.psoft.Util;
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Xml;

namespace ch.appl.psoft.Interface
{
    public class ExchangeHelper 
	{
		private long id;
		private string title;
		private string username;
		private string password;
		private string host;
		private string domain;
		private string path;
		private string href;

		System.Net.HttpWebRequest Request;
		System.Net.WebResponse Response;
		System.Net.CredentialCache MyCredentialCache;
		byte[] bytes = null;
		System.IO.Stream RequestStream = null;
		System.IO.Stream ResponseStream = null;
		XmlDocument ResponseXmlDoc = null;

		public static string DEFAULT_XMLREQ = "<?xml version=\"1.0\"?>"
			+ "<d:propfind xmlns:d='DAV:'>"
			+ "<d:prop><d:displayname/></d:prop>"
			+ "<d:prop><d:ishidden/></d:prop>"
			+ "<d:prop><d:isfolder/></d:prop>"
			+ "<d:prop><d:creationdate/></d:prop>"
			+ "</d:propfind>";

		#region Properties

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

		public string Path
		{
			get {return this.path;}
			set {this.path = value;}
		}

		public string URI 
		{
			get 
			{
				if(href == null) 
				{
					string uri = null;
					if (this.domain != null && this.domain != "") 
					{
						uri = "http://" + this.host + "." + this.domain + "/" + HttpUtility.UrlEncode(this.path) + "/";
					} 
					else 
					{
						uri = "http://" + this.host + "/" + HttpUtility.UrlEncode(this.path) + "/";
					}// if

					//HttpUtility.UrlEncode(uri);
					string s = uri.Replace("%2f","/");
					s = s.Replace("+","%20");
					href = s;
				}//if
				return href;
			}
		}

		public string Title 
		{
			get {return this.title;}
			set {this.title = value;}
		}

		public long ID 
		{
			get {return this.id;}
			set {this.id = value;}
		}

		#endregion

		public ExchangeHelper(long id, string title, string username, string password, string host, string domain, string path) 
		{
			this.id = id;
			this.title = title;
			this.username = username;
			this.password = password;
			this.host = host;
			this.domain = domain;
			this.path = path;
		}

		public ExchangeHelper(long id, string title, string username, string password, string href) 
		{
			this.id = id;
			this.title = title;
			this.username = username;
			this.password = password;
			this.href = href;
		}

		public static ExchangeFolder getExchangeFolder(HttpSessionState Session, long folderid) 
		{
			return ExchangeHelper.getExchangeHelper(Session, folderid).getFolder();
		}

		public static ExchangeHelper getExchangeHelper(HttpSessionState Session, long folderid) 
		{
			DBData _db;
			ExchangeHelper eh = null;

			_db = DBData.getDBData(Session);
			try 
			{
				_db.connect();

				DataTable exchangeFolderTable = _db.getDataTable("select ID," 
					+ _db.langAttrName("EXCHANGE_FOLDER", "TITLE") + ", "  
					+ _db.langAttrName("EXCHANGE_FOLDER", "DESCRIPTION") + ", "
					+ _db.langAttrName("EXCHANGE_FOLDER", "PATH") + ", "
					+ "EXCHANGE_ID"
					+ " from EXCHANGE_FOLDER WHERE ID = " + folderid);

				DataRow exchangeFolderRow = exchangeFolderTable.Rows[0];

				DataTable exchange = _db.getDataTable("select ID," 
					+ _db.langAttrName("EXCHANGE", "TITLE") + ", "  
					+ _db.langAttrName("EXCHANGE", "HOST") + ", "
					+ _db.langAttrName("EXCHANGE", "DOMAIN") + ", "
					+ _db.langAttrName("EXCHANGE", "PATH") + ", "
					+ _db.langAttrName("EXCHANGE", "USERNAME") + ", "
					+ _db.langAttrName("EXCHANGE", "PASSWORD")
					+ " from EXCHANGE WHERE ID = " + exchangeFolderRow["EXCHANGE_ID"]);

				foreach(DataRow row in exchange.Rows) 
				{
					long id = (long) row["ID"];
					string title = (string) row[1];
					string host = (string) row["HOST"];
					string domain = (string) row["DOMAIN"];
					string path = (string) row["PATH"];
					string username = (string) row["USERNAME"];
					string password = (string) row["PASSWORD"];
					
					eh = new ExchangeHelper(id, title, username, password, host, domain, path + "/" + exchangeFolderRow["PATH"]);
				}

			}
			catch (Exception ex) 
			{
				Logger.Log(ex, Logger.ERROR);
				throw new Exception("DoOnException. Unhandled exception occured in ExchangeHelper. " + ex.Message, ex);			
			}
			finally 
			{
				_db.disconnect();
			}

			return eh;
		}

		public static ArrayList getAllFolders(HttpSessionState Session) 
		{
			DBData _db;
			ArrayList al = new ArrayList();

			_db = DBData.getDBData(Session);
			try 
			{
				_db.connect();

				DataTable exchange = _db.getDataTable("select ID," 
					+ _db.langAttrName("EXCHANGE", "TITLE") + ", "  
					+ _db.langAttrName("EXCHANGE", "HOST") + ", "
					+ _db.langAttrName("EXCHANGE", "DOMAIN") + ", "
					+ _db.langAttrName("EXCHANGE", "PATH") + ", "
					+ _db.langAttrName("EXCHANGE", "USERNAME") + ", "
					+ _db.langAttrName("EXCHANGE", "PASSWORD")
					+ " from EXCHANGE order by " + _db.langAttrName("EXCHANGE", "TITLE"));

				foreach(DataRow row in exchange.Rows) 
				{
					long id = (long) row["ID"];
					string title = (string) row[1];
					string host = (string) row["HOST"];
					string domain = (string) row["DOMAIN"];
					string path = (string) row["PATH"];
					string username = (string) row["USERNAME"];
					string password = (string) row["PASSWORD"];

					al.AddRange(new ExchangeHelper(id, title, username, password, host, domain, path).getFolders());
				}

			}
			catch (Exception ex) 
			{
				Logger.Log(ex, Logger.ERROR);
				throw new Exception("DoOnException. Unhandled exception occured in ExchangeHelper. " + ex.Message, ex);			
			}
			finally 
			{
				_db.disconnect();
			}

			return al;
		}

		private ArrayList getItems() 
		{
			return getItems(DEFAULT_XMLREQ);
		}

		private ArrayList getItems(string requestBody) 
		{
			ArrayList al = new ArrayList();

			MyCredentialCache = new System.Net.CredentialCache();
			MyCredentialCache.Add( new System.Uri(URI),
				"NTLM",
				new System.Net.NetworkCredential(this.username, this.password)
				);

			// Create the HttpWebRequest object.
			Request = (System.Net.HttpWebRequest)HttpWebRequest.Create(URI);

			// Add the network credentials to the request.
			Request.Credentials = MyCredentialCache;

			// Specify the method.
			Request.Method = "PROPFIND";

			Request.Headers.Add("Depth", "1");

			if(requestBody == null || requestBody == "") 
			{
				bytes = Encoding.UTF8.GetBytes((string)DEFAULT_XMLREQ);
			} 
			else 
			{
				bytes = Encoding.UTF8.GetBytes((string)requestBody);
			}

			// Set the content header length.  This must be
			// done before writing data to the request stream.
			Request.ContentLength = bytes.Length;

			// Get a reference to the request stream.
			RequestStream = Request.GetRequestStream();

			// Write the request body to the request stream.
			RequestStream.Write(bytes, 0, bytes.Length);

			// Close the Stream object to release the connection
			// for further use.
			RequestStream.Close();

			// Set the content type header.
			Request.ContentType = "text/xml; charset=utf-8";

			// Send the PROPFIND method request and get the
			// response from the server.
			Response = (HttpWebResponse)Request.GetResponse();

			// Get the XML response stream.
			ResponseStream = Response.GetResponseStream();

			// Create the XmlDocument object from the XML response stream.
			ResponseXmlDoc = new XmlDocument();
			ResponseXmlDoc.Load(ResponseStream);

			XmlNodeList elemListDisplayname = ResponseXmlDoc.GetElementsByTagName("a:displayname");
			XmlNodeList elemListHref = ResponseXmlDoc.GetElementsByTagName("a:href");
			XmlNodeList elemListIsFolder = ResponseXmlDoc.GetElementsByTagName("a:isfolder");
			XmlNodeList elemListIsHidden = ResponseXmlDoc.GetElementsByTagName("a:ishidden");
			XmlNodeList elemListCreationDate = ResponseXmlDoc.GetElementsByTagName("a:creationdate");

			for (int i=0; i < elemListDisplayname.Count; i++) 
			{   
				string title = ((XmlNode) elemListDisplayname.Item(i)).InnerText;
				string href = ((XmlNode) elemListHref.Item(i)).InnerText;
				string isfolder = ((XmlNode) elemListIsFolder.Item(i)).InnerText;
				string ishidden = ((XmlNode) elemListIsHidden.Item(i)).InnerText;
				string creationdate = ((XmlNode) elemListCreationDate.Item(i)).InnerText;

				ExchangeItem ef = new ExchangeItem(id, this.title, title, href, ishidden, isfolder, username, password, host, domain, creationdate);

				al.Add(ef);
			}//for

			//clean up
			ResponseStream.Close();
			Response.Close();

			return al;
		}//getItems

		public ArrayList getFolders() 
		{
			ArrayList retVal = new ArrayList();
			
			try 
			{
				ArrayList items = getItems();
			
				foreach (ExchangeItem ei in items) 
				{
					if(ei.Href != URI && ei.Isfolder && !ei.Ishidden) 
					{
						retVal.Add(new ExchangeFolder(ei));
					}//if
				}
			} 
			catch (Exception) 
			{
				//ignore
			}
			return retVal;
		}//getFolders

		public ExchangeFolder getFolder() 
		{
			ExchangeFolder retVal = null;
			
			try 
			{
				ArrayList items = getItems();
			
				foreach (ExchangeItem ei in items) 
				{
					if(ei.Href == URI && ei.Isfolder && !ei.Ishidden) 
					{
						retVal = new ExchangeFolder(ei);
						break;
					}//if
				}
			} 
			catch (Exception ex) 
			{
				throw new ExchangeException("noconnection " + ex.Message);
			}
			return retVal;
		}//getFolders

		public ArrayList getMessages() 
		{
			ArrayList items = getItems();
			ArrayList retVal = new ArrayList();
			foreach (ExchangeItem ei in items) 
			{
				if(ei.Href != URI && !ei.Isfolder && !ei.Ishidden) 
				{
					ExchangeMessage em = new ExchangeMessage(ei);
					setAdditionalInfo(em, ExchangeMessage.FROM, ExchangeMessage.HASATTACHEMENTS);//, ExchangeMessage.TEXTDESCRIPTION, ExchangeMessage.HTMLDESCRIPTION);
					if(em.getAdditionalAttribute(ExchangeMessage.HASATTACHEMENTS) == "1") 
					{
						em.setAdditionalAttribute(ExchangeMessage.NROFATTACHEMENTS, Convert.ToString(getNumberOfAttachements(em.Href)));
					} 
					else 
					{
						em.setAdditionalAttribute(ExchangeMessage.NROFATTACHEMENTS, "0");
					}
					retVal.Add(em);
				}//if
			}
			return retVal;
		}//getFolders

		public ExchangeMessage getMessage(string href) 
		{
			ExchangeMessage retVal = null;
			
			try 
			{
				ArrayList items = getItems();
			
				foreach (ExchangeItem ei in items) 
				{
					if(ei.Href == href && !ei.Isfolder && !ei.Ishidden) 
					{
						retVal = new ExchangeMessage(ei);
						setAdditionalInfo(retVal, ExchangeMessage.FROM, ExchangeMessage.TEXTDESCRIPTION, ExchangeMessage.HTMLDESCRIPTION);
						break;
					}//if
				}
			} 
			catch (Exception) 
			{
				//ignore
			}
			return retVal;
		}

		public Stream getFileStream(string href) 
		{
			MyCredentialCache = new System.Net.CredentialCache();
			MyCredentialCache.Add( new System.Uri(URI),
				"NTLM",
				new System.Net.NetworkCredential(this.username, this.password)
				);

			Request = (System.Net.HttpWebRequest)HttpWebRequest.Create(href);
			Request.Credentials = MyCredentialCache;
			Request.Method = "GET";
			Request.Headers.Add("Translate","f");
			Response = (HttpWebResponse)Request.GetResponse();
			return Response.GetResponseStream();
		}//getEMLFileStream

		public void DownloadMessageAsEML(string href, string file) 
		{
			ResponseStream = getFileStream(href);

			FileStream fileStream = new FileStream(file,FileMode.Create);
				
			byte [] buffer = new byte [4096];
			int len = 0;

			while ((len = ResponseStream.Read(buffer,0,buffer.Length)) > 0 )
			{
				fileStream.Write(buffer,0,len);
			}

			fileStream.Close();
			ResponseStream.Close();
		}//DownloadMessageAsEML

		private void setAdditionalInfo(ExchangeItem ei, params string[] attributes) 
		{
			MyCredentialCache = new System.Net.CredentialCache();
			MyCredentialCache.Add( new System.Uri(ei.Href),
				"NTLM",
				new System.Net.NetworkCredential(this.username, this.password)
				);

			// Create the HttpWebRequest object.
			Request = (System.Net.HttpWebRequest)HttpWebRequest.Create(ei.Href);

			// Add the network credentials to the request.
			Request.Credentials = MyCredentialCache;

			// Specify the method.
			Request.Method = "PROPFIND";

			Request.Headers.Add("Depth", "1");

			Request.Headers.Add("Accept-Encoding", "gzip, deflate");

			Request.KeepAlive = true;

			Request.Headers.Add("Cache-Control", "no-cache");

			//TODO:
			//If you know a better way, to retrieve only that elements you want, write
			//it down. At the moment, you just catch everthing.
			Request.ContentLength = 0;

			// Set the content type header.
			Request.ContentType = "text/xml";

			// Send the PROPFIND method request and get the
			// response from the server.
			Response = (HttpWebResponse)Request.GetResponse();

			// Get the XML response stream.
			ResponseStream = Response.GetResponseStream();

			// Create the XmlDocument object from the XML response stream.
			ResponseXmlDoc = new XmlDocument();
			ResponseXmlDoc.Load(ResponseStream);

			for(int i = 0; i < attributes.Length; i++) 
			{
				ei.setAdditionalAttribute(attributes[i], ResponseXmlDoc.GetElementsByTagName(attributes[i]).Item(0).InnerText);
			}
		}

		public int getNumberOfAttachements(string href)
		{
			// Create a new CredentialCache object and fill it with the network
			// credentials required to access the server.
			MyCredentialCache = new System.Net.CredentialCache();
			MyCredentialCache.Add( new System.Uri(href),
				"NTLM",
				new System.Net.NetworkCredential(this.username, this.password)
				);

			// Create the HttpWebRequest object.
			Request = (System.Net.HttpWebRequest)HttpWebRequest.Create(href);

			// Add the network credentials to the request.
			Request.Credentials = MyCredentialCache;

			// Specify the method.
			Request.Method = "X-MS-ENUMATTS";

			// Send the X-MS-ENUMATTS method request and get the
			// response from the server.
			Response = (HttpWebResponse)Request.GetResponse();

			// Get the XML response stream.
			ResponseStream = Response.GetResponseStream();

			// Create the XmlDocument object from the XML response stream.
			ResponseXmlDoc = new System.Xml.XmlDocument();

			// Load the XML response stream.
			ResponseXmlDoc.Load(ResponseStream);

			// Get the root node.
			XmlElement root = ResponseXmlDoc.DocumentElement;

			// Create a new XmlNamespaceManager.
			XmlNamespaceManager nsmgr = new System.Xml.XmlNamespaceManager(ResponseXmlDoc.NameTable);

			// Add the DAV: namespace, which is typically assigned the a: prefix
			// in the XML response body.  The namespaceses and their associated
			// prefixes are listed in the attributes of the DAV:multistatus node
			// of the XML response.
			nsmgr.AddNamespace("a", "DAV:");

			// Add the http://schemas.microsoft.com/mapi/proptag/ namespace, which
			// is typically assigned the d: prefix in the XML response body.
			nsmgr.AddNamespace("d", "http://schemas.microsoft.com/mapi/proptag/");

			// Use an XPath query to build a list of the DAV:propstat XML nodes,
			// corresponding to the returned status and properties of
			// the file attachment(s).
			XmlNodeList PropstatNodes = root.SelectNodes("//a:propstat", nsmgr);

			// Use an XPath query to build a list of the DAV:href nodes,
			// corresponding to the URIs of the attachement(s) on the message.
			// For each DAV:href node in the XML response, there is an
			// associated DAV:propstat node.
			XmlNodeList HrefNodes = root.SelectNodes("//a:href", nsmgr);

			// Clean up.
			ResponseStream.Close();
			Response.Close();

			return HrefNodes.Count;
		}//getNumberOfAttachements

		public ArrayList getAttachements(string href)
		{
			ArrayList al = new ArrayList();

			// Create a new CredentialCache object and fill it with the network
			// credentials required to access the server.
			MyCredentialCache = new System.Net.CredentialCache();
			MyCredentialCache.Add( new System.Uri(href),
				"NTLM",
				new System.Net.NetworkCredential(this.username, this.password)
				);

			// Create the HttpWebRequest object.
			Request = (System.Net.HttpWebRequest)HttpWebRequest.Create(href);

			// Add the network credentials to the request.
			Request.Credentials = MyCredentialCache;

			// Specify the method.
			Request.Method = "X-MS-ENUMATTS";

			// Send the X-MS-ENUMATTS method request and get the
			// response from the server.
			Response = (HttpWebResponse)Request.GetResponse();

			// Get the XML response stream.
			ResponseStream = Response.GetResponseStream();

			// Create the XmlDocument object from the XML response stream.
			ResponseXmlDoc = new System.Xml.XmlDocument();

			// Load the XML response stream.
			ResponseXmlDoc.Load(ResponseStream);

			// Get the root node.
			XmlElement root = ResponseXmlDoc.DocumentElement;

			// Create a new XmlNamespaceManager.
			XmlNamespaceManager nsmgr = new System.Xml.XmlNamespaceManager(ResponseXmlDoc.NameTable);

			// Add the DAV: namespace, which is typically assigned the a: prefix
			// in the XML response body.  The namespaceses and their associated
			// prefixes are listed in the attributes of the DAV:multistatus node
			// of the XML response.
			nsmgr.AddNamespace("a", "DAV:");

			// Add the http://schemas.microsoft.com/mapi/proptag/ namespace, which
			// is typically assigned the d: prefix in the XML response body.
			nsmgr.AddNamespace("d", "http://schemas.microsoft.com/mapi/proptag/");

			// Use an XPath query to build a list of the DAV:propstat XML nodes,
			// corresponding to the returned status and properties of
			// the file attachment(s).
			XmlNodeList PropstatNodes = root.SelectNodes("//a:propstat", nsmgr);

			// Use an XPath query to build a list of the DAV:href nodes,
			// corresponding to the URIs of the attachement(s) on the message.
			// For each DAV:href node in the XML response, there is an
			// associated DAV:propstat node.
			XmlNodeList HrefNodes = root.SelectNodes("//a:href", nsmgr);

			// Attachments found?
			if(HrefNodes.Count > 0)
			{
				// Iterate through the attachment properties.
				for(int i=0;i<HrefNodes.Count;i++)
				{
					// Use an XPath query to get the DAV:status node from the DAV:propstat node.
					XmlNode StatusNode = PropstatNodes[i].SelectSingleNode("a:status", nsmgr);

					// Check the status of the attachment properties.
					if(StatusNode.InnerText == "HTTP/1.1 200 OK")
					{					
						ExchangeAttachement attachement = new ExchangeAttachement();
						attachement.Href = HrefNodes[i].InnerText;

						// Get the CdoPR_ATTACH_FILENAME_W MAPI property tag,
						// corresponding to the attachment file name.  The
						// http://schemas.microsoft.com/mapi/proptag/ namespace is typically
						// assigned the d: prefix in the XML response body.
						attachement.Filename = PropstatNodes[i].SelectSingleNode("a:prop/d:x3704001f", nsmgr).InnerText;

						// Get the CdoPR_ATTACH_EXTENSION_W MAPI property tag,
						// corresponding to the attachment file extension.
						attachement.Fileextension = PropstatNodes[i].SelectSingleNode("a:prop/d:x3703001f", nsmgr).InnerText;

						// Get the CdoPR_ATTACH_SIZE MAPI property tag,
						// corresponding to the attachment file size.
						attachement.Filesize = Convert.ToString(Math.Ceiling(Convert.ToInt32(PropstatNodes[i].SelectSingleNode("a:prop/d:x0e200003", nsmgr).InnerText)/1024.0)) + "K";
						
						attachement.Mimetype = PropstatNodes[i].SelectSingleNode("a:prop/d:x370e001f", nsmgr).InnerText;

						al.Add(attachement);
					}//if
				}//for
			}//if

			// Clean up.
			ResponseStream.Close();
			Response.Close();

			return al;
		}//getNumberOfAttachements
	}
}

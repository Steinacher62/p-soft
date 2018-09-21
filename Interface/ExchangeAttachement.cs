namespace ch.appl.psoft.Interface
{
    /// <summary>
    /// Summary description for ExchangeAttachement.
    /// </summary>
    public class ExchangeAttachement
	{

		private string href;
		private string mimetype;
		private string filename;
		private string filesize;
		private string fileextension;

		#region Properties

		public string Href
		{
			get {return this.href;}
			set {this.href = value;}
		}

		public string Mimetype
		{
			get {return this.mimetype;}
			set {this.mimetype = value;}
		}

		public string Filename
		{
			get {return this.filename;}
			set {this.filename = value;}
		}

		public string Filesize
		{
			get {return this.filesize;}
			set {this.filesize = value;}
		}

		public string Fileextension
		{
			get {return this.fileextension;}
			set {this.fileextension = value;}
		}

		#endregion

		public ExchangeAttachement()
		{
		}
	}
}

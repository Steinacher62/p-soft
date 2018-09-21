using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using System.Collections;
using System.Web.SessionState;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Common
{
    /// <summary>
    /// Summary description for WikiBoxCtrl.
    /// </summary>
    public class WikiBoxCtrl : Table
	{

		private ArrayList _items = new ArrayList();
		private ArrayList _iitems = new ArrayList();
		private System.Web.UI.WebControls.TextBox _box = new TextBox();
		private string _UID;
		private string _fromUID;
		private string _xID;
		private string _xParentID;
		private string _tableName;
		private HttpSessionState _session;
		private LanguageMapper _mapper;
		private DBData _db;

		public static void SET_TO_UID(HttpSessionState session,long uid)
		{
			session["WIKIBOXTOUID"] = uid;
		}

		public WikiBoxCtrl() : base()
		{
			init();
		}

		#region Properities
		public string UID
		{			
			get { return _UID; }
			set 
			{
				_UID = value;
				if (_db != null)
				{
					long id = _db.UID2ID(long.Parse(_UID));					
					ObjectLink_TableName = _db.UID2Tablename(long.Parse(_UID));
					ObjectLink_FromUID = _UID;
					switch (ObjectLink_TableName)
					{
						case "THEME":
							ObjectLink_xParentID = _db.Theme.getParentID(id).ToString();
							if (ObjectLink_xParentID == "-1")
							{
								ObjectLink_xID = _db.Theme.getKnowledgeID(id).ToString();
							}
							else
							{
								ObjectLink_xID = id.ToString();
							}
							break;
					}

				}
			}
		}
		public string ObjectLink_FromUID
		{			
			get { return _fromUID; }
			set { _fromUID = value; }
		}
		public string ObjectLink_xID
		{			
			get { return _xID; }
			set { _xID = value; }
		}
		public string ObjectLink_xParentID
		{			
			get { return _xParentID; }
			set { _xParentID = value; }
		}
		public string ObjectLink_TableName
		{
			get { return _tableName; }
			set { _tableName = value; }
		}
		public TextBox Box
		{
			get { return _box; }
			set { _box = value; }
		}
		public string Text
		{
			get { return Box.Text; }
			set { Box.Text = value; }
		}
		public HttpSessionState Session
		{
			set { 
				_session = value;
				_mapper = LanguageMapper.getLanguageMapper(_session);
				_db = DBData.getDBData(_session);
			}
		}
		public string WikiBoxContent
		{
			get { 
				string ret = "";
				if (_session != null && _session["WIKIBOXCONTENT"] != null)
				{
					ret = _session["WIKIBOXCONTENT"].ToString();
					if (_session["WIKIBOXTOUID"] != null)
					{
						ret = ret.Replace("WikiBoxReplaceString",_session["WIKIBOXTOUID"].ToString());
					}
				}
				return ret;
			}
			set 
			{
				if(_session != null)
				{
					_session["WIKIBOXCONTENT"] = value;
				}
			}
		}
		#endregion

		#region PublicMethods
		public void Refresh()
		{
			RemoveButtonAll();
			loadButtons();
			if(_session != null)
			{
				_session["WIKIBOXCONTENT"] = "";
				_session["WIKIBOXTOUID"] = "";
			}
		}

		public void setSession(HttpSessionState Session)
		{
			HttpSessionState session = Session;
		}
		
		public void SetWikiBoxText(string text)
		{
			if (WikiBoxContent == "")
			{
				Text = text;
			}
			else
			{
				Text = WikiBoxContent;
			}
		}
		
		#endregion


		private Image GetButton(int index)
		{
			return (Image) _items[index];												 
		}
		private Image GetIButton(int index)
		{
			return (ImageButton) _iitems[index];												 
		}

		private int CountButtons
		{
			get {return _items.Count;}
		}
		private int CountIButtons
		{
			get {return _iitems.Count;}
		}

		private int AddButton(Image btn)
		{
			return _items.Add(btn);
		}

		private int AddButton(ImageButton btn)
		{
			return _iitems.Add(btn);
		}

		private void RemoveButton(Button btn)
		{
			_items.Remove(btn);
		}

		private void RemoveButtonAt(int index)
		{
			_items.RemoveAt(index);
		}

		private void RemoveButtonAll()
		{
			_items.Clear();
		}

		private void init() 
		{
			base.Load += new System.EventHandler(this.onLoad);			
			base.Width = Unit.Percentage(100);
			base.CellPadding = 0;
			base.CellSpacing = 0;
			
			Box.TextMode = TextBoxMode.MultiLine;
			Box.Width = Unit.Percentage(100);
			Box.Rows = 25;
			Box.Columns = 100;
			Box.ID = "WikiBox";
		}

		private void objectlink_button_Click(object sender, System.Web.UI.ImageClickEventArgs e){
			WikiBoxContent = Box.Text; // keeping the unsaved changes
			
			string nextUrl = "";
			switch (ObjectLink_TableName)
			{
				case "THEME":
					if (ObjectLink_xParentID == "-1")
					{
						nextUrl = psoft.Knowledge.EditKnowledge.GetURL("mode","edit","knowledgeID",ObjectLink_xID);
					}
					else
					{
						nextUrl = psoft.Knowledge.EditTheme.GetURL("mode","edit","themeID",ObjectLink_xID);
					}
					break;
			}
			Page.Response.Redirect(psoft.Common.AddUIDAssignments.GetURL("fromUID",ch.psoft.Util.Validate.GetValid(ObjectLink_FromUID,"-1"),"nextURL",nextUrl), false);
		}

		private void loadButtons()
		{
			Image btn = new Image();
			btn.ToolTip = _mapper.get("wikibox","bold");
			btn.ImageUrl = "../../images/wikibox/button_bold.png";
			btn.ID = "bold_button";
			btn.Attributes.Add("onClick","insertTags('\\\'\\\'\\\'', '\\\'\\\'\\\'', '"+_mapper.get("wikibox","bold")+"','WikiBox')");
			AddButton(btn);

			btn = new Image();
			btn.ToolTip = _mapper.get("wikibox","italic");
			btn.ImageUrl = "../../images/wikibox/button_italic.png";
			btn.ID = "italic_button";
			btn.Attributes.Add("onClick","insertTags('\\\'\\\'', '\\\'\\\'', '"+_mapper.get("wikibox","italic")+"','WikiBox')");
			AddButton(btn);

			btn = new Image();
			btn.ToolTip = _mapper.get("wikibox","headline");
			btn.ImageUrl = "../../images/wikibox/button_headline.png";
			btn.ID = "headline_button";
			btn.Attributes.Add("onClick","insertTags('=#', '\\n', '"+_mapper.get("wikibox","headline")+"','WikiBox')");
			AddButton(btn);

			btn = new Image();
			btn.ToolTip = _mapper.get("wikibox","hr");
			btn.ImageUrl = "../../images/wikibox/button_hr.png";
			btn.ID = "hr_button";
			btn.Attributes.Add("onClick","insertTags('----\\n', '', '','WikiBox')");
			AddButton(btn);

			btn = new Image();
			btn.ToolTip = _mapper.get("wikibox","image");
			btn.ImageUrl = "../../images/wikibox/button_image.png";
			btn.ID = "image_button";
			btn.Attributes.Add("onClick","insertTags('[[Bild:', '|thumb]]', '"+_mapper.get("wikibox","image")+"','WikiBox')");
			AddButton(btn);

			btn = new Image();
			btn.ToolTip = _mapper.get("wikibox","extlink");
			btn.ImageUrl = "../../images/wikibox/button_extlink.png";
			btn.ID = "extlink_button";
			btn.Attributes.Add("onClick","insertTags('[', ']', 'http://"+_mapper.get("wikibox","extlinksample")+" "+_mapper.get("wikibox","extlink")+"','WikiBox')");
			AddButton(btn);

			if (ch.psoft.Util.Validate.GetValid(ObjectLink_FromUID,"-1") == "-1")
			{
				btn = new Image();
				btn.ToolTip = _mapper.get("wikibox","objectlink");
				btn.ImageUrl = "../../images/wikibox/button_link.png";
				btn.ID = "link_button";
				btn.Attributes.Add("onClick","insertTags('[[UID:', ']]', '0000|"+_mapper.get("wikibox","objectlink")+"','WikiBox')");
				AddButton(btn);
			}
			else
			{
				ImageButton ibtn = new ImageButton();
				ibtn.ToolTip = _mapper.get("wikibox","objectlink");
				ibtn.ImageUrl = "../../images/wikibox/button_link.png";
				ibtn.ID = "link_button";
				ibtn.Attributes.Add("onClick","insertTags('[[UID:', ']]', 'WikiBoxReplaceString','WikiBox')");
				ibtn.Click += new System.Web.UI.ImageClickEventHandler(objectlink_button_Click);
				AddButton(ibtn);				
			}
			
			//			btn = new Image();
			//			btn.ToolTip = "nowiki";
			//			btn.ImageUrl = "../../images/wikibox/button_nowiki.png";
			//			btn.ID = "nowiki_button";
			//			btn.Attributes.Add("onClick","insertTags('<nowiki>', '</nowiki>', 'nowiki','WikiBox')");
			//			AddButton(btn);


		}

		private void onLoad(object sender, System.EventArgs args) 
		{
			TableRow r = null;
			TableCell c = null;
			
			r = new TableRow();
			base.Rows.Add(r);
			
			c = new TableCell();
			for (int i = 0; i < CountButtons; i++)
			{
				c.Controls.Add(GetButton(i));
			}
			for (int ii = 0; ii < CountIButtons; ii++)
			{
				c.Controls.Add(GetIButton(ii));
			}
			r.Cells.Add(c);

			r = new TableRow();
			base.Rows.Add(r);

			c = new TableCell();
			c.Controls.Add(Box);
			r.Cells.Add(c);
		}
	}
}

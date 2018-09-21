using System;
using System.Collections;
using System.Text;

namespace ch.appl.psoft.LayoutControls
{
    public class LinkItem : IComparable
	{

		private string _caption = "";
		private string _link = "";
		private string _params = "";
        private string _target = "_self";

		public LinkItem(string caption, string link) : this(caption, link, "") {}

        public LinkItem(string caption, string link, string parameters) : this(caption, link, parameters, "") {}

		public LinkItem(string caption, string link, string parameters, string target)
		{
			_caption = caption;
			_link = link;
			_params = parameters;
            if (target != ""){
                _target = target;
            }
		}

		public string Caption
		{
			get {return _caption;}
		}

		public string Link
		{
			get {return _link;}
		}

        public string Params {
            get {return _params;}
        }

        public string Target {
            get {return _target;}
        }

        public override string ToString()
		{
			if(_link.Length > 0)
			{
				StringBuilder str = new StringBuilder();
				str.Append("<a href=\"");
				if (!Link.ToLower().StartsWith("javascript") && !Link.StartsWith(Global.Config.baseURL) && !Link.StartsWith("#"))
				{
					str.Append(Global.Config.baseURL);
				}
				str.Append(Link);
				str.Append("\" target=\"" + Target + "\" class=\"bold\" ");
				str.Append(Params);
				str.Append(">");
				str.Append(Caption);
				str.Append(" >>");
				str.Append("</a>");
				return str.ToString();
			}
			else
			{
				return Caption;
			}

		}

        public int CompareTo(object obj){
            LinkItem otherLinkItem = obj as LinkItem;
            if (otherLinkItem != null){
                return _caption.CompareTo(otherLinkItem._caption);
            }
            else{
                return _caption.CompareTo(obj);
            }
        }
    }


	public class LinkSet
	{
		private string _linkSetName = "";
		private ArrayList _items = new ArrayList();

		public LinkSet(string linkSetName)
		{
			_linkSetName = linkSetName;
		}

		public string linkSetName
		{
			get {return _linkSetName;}
			set {_linkSetName = value;}
		}

		public int Count
		{
			get {return _items.Count;}
		}

		public int AddItem(LinkItem item)
		{
			return _items.Add(item);
		}

		public int AddItem(string itemCaption, string itemLink)
		{
			return AddItem(new LinkItem(itemCaption, itemLink));
		}

		public int AddItem(string itemCaption, string itemLink, string parameters, string target)
		{
			return AddItem(new LinkItem(itemCaption, itemLink, parameters, target));
		}

        public void Sort(){
            _items.Sort();
        }

		public override string ToString()
		{
			StringBuilder str = new StringBuilder();
			str.Append("<li>");
            bool isFirst = true;
			if (_linkSetName != "")
			{
				str.Append(_linkSetName);
                isFirst = false;
			}
			foreach (LinkItem _item in _items)
			{
                if (isFirst)
                    isFirst = false;
                else
                    str.Append("<br>");
                str.Append(_item.ToString());
			}
			str.Append("</li>");
			return str.ToString();
		}
	}

	public class LinkSets
	{
		private ArrayList _sets = new ArrayList();

		public LinkSets()
		{
		}

		public int Count
		{
			get {return _sets.Count;}
		}

		public LinkSet GetLinkSet(string linkSetName)
		{
			LinkSet result = null;
			foreach (LinkSet linkSet in _sets)
				if (linkSet.linkSetName.ToUpper() == linkSetName.ToUpper())
				{
					result = linkSet;
					break;
				}
			return result;
		}

		public int Add(string linkSetName, string itemCaption, string itemLink)
		{
			return Add(linkSetName, itemCaption, itemLink, "");
		}

        public int Add(string linkSetName, string itemCaption, string itemLink, string parameters) {
            return Add(linkSetName, itemCaption, itemLink, parameters, "");
        }

        public int Add(string linkSetName, string itemCaption, string itemLink, string parameters, string target)
		{
			LinkSet linkSet = GetLinkSet(linkSetName);
			if (linkSet == null)
			{
				linkSet = new LinkSet(linkSetName);
				_sets.Add(linkSet);
			}
			linkSet.AddItem(itemCaption, itemLink, parameters, target);
			return _sets.IndexOf(linkSet);
		}

        public void Sort(string linkSetName){
			LinkSet linkSet = GetLinkSet(linkSetName);
            if (linkSet != null) {
                linkSet.Sort();
            }
        }

		public override string ToString()
		{
			StringBuilder str = new StringBuilder();
			str.Append("<ul>");
            bool isFirst = true;
            foreach (LinkSet linkSet in _sets)
            {
                if (isFirst)
                    isFirst = false;
                else
                    str.Append("<br><br>");
                str.Append(linkSet.ToString());
            }
			str.Append("</ul>");
			return str.ToString();
		}
	}
}
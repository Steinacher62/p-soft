using System.Collections.Generic;
using System.Text;
using System.Web;

namespace ch.appl.psoft.Common
{

    public class BreadcrumbItem
    {

        private string _link = "";
        private string _name = "";
        private string _caption = "";

        private BreadcrumbItem _parent = null;
        private BreadcrumbItem _child = null;

        public const string BREAD_CRUMB_ITEM = "__BREAD_CRUMB_ITEM";

        public BreadcrumbItem(string name, string caption, string link)
        {
            _name = name;
            _caption = caption;
            _link = link;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Caption
        {
            get { return _caption; }
        }

        public string Link
        {
            get { return _link; }
        }

        public BreadcrumbItem LastChild
        {
            get
            {
                BreadcrumbItem item = this;
                while (item._child != null)
                {
                    item = item._child;
                }
                return item;
            }
        }

        public BreadcrumbItem FirstChild
        {
            get
            {
                BreadcrumbItem item = this;
                while (item._parent != null)
                {
                    item = item._parent;
                }
                return item._child;
            }
        }

        public int Count
        {
            get { return getCount(); }
        }

        public List<listItem> Items
        {
            get { return getItems(); }
        }



        private int getCount()
        {
            int count = 0;
            BreadcrumbItem item = this;
            while (item._child != null)
            {
                item = item._child;
                count++;
            }
            return count;
        }


        private List<listItem> getItems()
        {

            int count = 0;
            var _items = new List<listItem>();
            BreadcrumbItem item = this;
            while (item._child != null)
            {
                item = item._child;
                listItem li = new listItem();
                li.Caption = item.Caption;
                li.Link = item.Link;
                _items.Insert(0, li);
                count++;
            }

            if (item.FirstChild == null)
            {
                listItem li = new listItem();
                li.Caption = item.Caption;
                li.Link = item.Link;
                _items.Add(li);
            }
            if (!(item.FirstChild == null))
            {
                item = item.FirstChild._parent;
                listItem li = new listItem();
                li.Caption = item.Caption;
                li.Link = item.Link;
                _items.Add(li);
            }

            

            return _items;
        }
        public class listItem
        {
            public string Caption { get; set; }
            public string Link { get; set; }
        }



        public void AddChild(BreadcrumbItem item)
        {
            _child = item;
            item._parent = this;
        }

        public void AddChild(string name, string caption, string link)
        {
            AddChild(new BreadcrumbItem(name, caption, link));
        }

        public bool IsSameChild(BreadcrumbItem item)
        {
            return (_child != null) ? (_child._name == item._name) : false;
        }

        public void SetBreadcrumbItem(BreadcrumbItem item)
        {
            if (_name == item._name)
            {
                _caption = item._caption;
                _link = item._link;
                if (_child != null)
                {
                    _child._parent = null;
                    _child = null;
                }
            }
            else if (IsSameChild(item))
            {
                _child._parent = null;
                AddChild(item);
            }
            else if (_child != null)
                _child.SetBreadcrumbItem(item);
            else
                AddChild(item);
        }

        public void RemoveLastChild()
        {
            BreadcrumbItem item = LastChild;

            if (item._parent != null)
            {
                item._parent._child = null;
                item._parent = null;
            }
        }

        public void RemoveFirstChild()
        {
            BreadcrumbItem item = FirstChild;
            BreadcrumbItem deleteItem;

            while (item._parent != null)
            {
                item = item._parent;

            }

            deleteItem = item._child;
            item._child = item._child._child;
            deleteItem = null;
        }


        public override string ToString()
        {
            StringBuilder _str = new StringBuilder();
            _str.Append("<a href=\"");
            _str.Append(_link);
            _str.Append("\" class=\"bold\">");
            _str.Append(HttpUtility.HtmlEncode(_caption));
            _str.Append("</a>");
            if (_child != null)
            {
                _str.Append("&nbsp;-&nbsp;");
                _str.Append(_child.ToString());
            }
            return _str.ToString();
        }
    }
}

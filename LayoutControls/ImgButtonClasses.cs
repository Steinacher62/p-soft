using System.Collections;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.LayoutControls
{
    /// <summary>
    /// Summary description for ImgButtonClasses.
    /// </summary>
    public class ImgButtonList : IEnumerable
	{
		private ArrayList _items = new ArrayList();

		public ImgButtonList() {}

		public int Count 
		{
			get {return _items.Count;}
		}

		public int Add(ImgButtonControl imageButtonControl)
		{
			return _items.Add(imageButtonControl);
		}

		public void Clear()
		{
			_items.Clear();
		}

		public void Remove(ImgButtonControl imageButtonControl)
		{
			_items.Remove(imageButtonControl);
		}

		public void RemoveAt(int index)
		{
			_items.RemoveAt(index);
		}

		public System.Web.UI.WebControls.Table Parse()
		{
			System.Web.UI.WebControls.Table _result = new System.Web.UI.WebControls.Table();
			System.Web.UI.WebControls.TableRow _row = new TableRow();
			_result.Width = Unit.Percentage(100);

			foreach (ImgButtonControl _img in _items)
			{
				System.Web.UI.WebControls.TableCell _cell = new TableCell();
				_cell.Controls.Add(_img);
				_cell.HorizontalAlign = HorizontalAlign.Center;
				_row.Cells.Add(_cell);
			}
			_result.Rows.Add(_row);
			return _result;
		}

		#region IEnumerable Members
		public IEnumerator GetEnumerator()
		{
			return _items.GetEnumerator();
		}
		#endregion
	}
}

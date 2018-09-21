using ch.appl.psoft.db;
using ch.appl.psoft.Interface;
using ch.psoft.db;
using System;
using System.Data;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Common
{
    /// <summary>
    /// Summary description for DetailBuilder.
    /// </summary>
    public class DetailBuilder {
        private DBData _db;
        private Table _detailTab = null;
        string _viewName = "";
        private LanguageMapper _map;
        private bool _checkOrder = false;
        private bool _displayNull = true;
        /// <summary>
        /// Detail cell event. Wird nach add cell gerufen
        /// </summary>
        public event AddPropertyHandler detailRow = null;
        /// <summary>
        /// Set check columnorder in select (false)
        /// </summary>
        public bool checkOrder {
            set { _checkOrder = value; }
        }
        /// <summary>
        /// Set display null-values or not (true)
        /// </summary>
        public bool displayNull {
            set { _displayNull = value; }
        }
        /// <summary>
        /// Bildet eine Detail-tabelle
        /// </summary>
        /// <param name="db">DB</param>
        /// <param name="table">Tabelle mit erweiterter Kolonnendefinition</param>
        /// <param name="detailTab">Web Tabelle mit such-attributen</param>
        /// <param name="map">Sprach mapping</param>
        /// <param name="view">fak. view to load</param>
        public void load(DBData db, DataTable table, Table detailTab, LanguageMapper map, params string[] param) {
            string view = param.Length > 0 ? param[0] : "";
            
            _detailTab = detailTab;
            _map = map;
            _db = db;
            _viewName = view == "" ? table.TableName : view;

            _db.scanRow += new ScanActionRowHandler(this.scanDetailRow);
            _db.scanCell += new ScanActionCellHandler(this.scanDetail);
            _db.scanTableData(table,_viewName,(int) SQLColumn.Visibility.DETAIL, DBData.AUTHORISATION.READ, DBData.APPLICATION_RIGHT.COMMON, _checkOrder, false);
            _db.scanRow -= new ScanActionRowHandler(this.scanDetailRow);
            _db.scanCell -= new ScanActionCellHandler(this.scanDetail);
        }
        private bool scanDetailRow(DataTable table, DataRow row, int rowNumber, int rowAuthorisations) {
            return rowNumber == 1;
        }
        private void scanDetail(DataTable table, DataRow row, int rowNumber, DataColumn col, int columnNumber) {
            if (columnNumber < 0) return;
            if (!this._displayNull && SQLColumn.IsNull(row[col])) return;

            TableRow r = new TableRow();
            TableCell c = new TableCell();;
            
            r.CssClass = "Detail";
            r.VerticalAlign = VerticalAlign.Top;

            _detailTab.Rows.Add(r);
            r.Cells.Add(c);
            c.CssClass = "Detail_Label";
            c.Text = _map.get(_viewName,col.ColumnName);
            c.Wrap = false;
            c = new TableCell();
            r.Cells.Add(c);
            c.CssClass = "Detail_Value";
            if (col.ExtendedProperties["In"] != null)
                c.Text = DBColumn.LookupIn(col,row[col],true);
            else
                c.Text = _db.GetDisplayValue(col,row[col],true);

            string unit = (string) col.ExtendedProperties["Unit"];
            if (unit != "") c.Text += " " + unit;

            Type controlType = (Type) col.ExtendedProperties["DetailControlType"];
            if (controlType != null) {
                string text = c.Text;
                WebControl ctrl = (WebControl) controlType.GetConstructor(Type.EmptyTypes).Invoke(null);

                c.Text = null;
                if (ctrl is Label) (ctrl as Label).Text = text;
                else if (ctrl is EMailLink) {
                    (ctrl as EMailLink).Text = text;
                    (ctrl as EMailLink).NavigateUrl = text;
                }
                else if (ctrl is HyperLink) {
                    (ctrl as HyperLink).Text = text;
                    (ctrl as HyperLink).NavigateUrl = text;
                }
                else if (ctrl is CheckBox) {
                    int val = DBColumn.GetValid(row[col], 0);
                    CheckBox cb = ctrl as CheckBox;

                    cb.Checked = val != 0;
                    cb.Enabled = false;
                }
                else ctrl = null;
                if (ctrl != null) c.Controls.Add(ctrl);
                else c.Text = text;
            }
            else{
                string contextLink = col.ExtendedProperties["ContextLink"] as string;
                if (contextLink != null && contextLink != ""){
                    contextLink = replaceURLPlaceholders(contextLink, row);
                    HyperLink url = new HyperLink();                               

                    url.NavigateUrl = contextLink;
                    url.CssClass = c.CssClass;
                    url.Text = c.Text;
                    url.Target = col.ExtendedProperties["ContextLinkTarget"] as string;
                    c.Controls.Add(url);
                }
            }
            onDetailRow(row,col,r);
        }
        private void onDetailRow (DataRow row, DataColumn column, TableRow r) {
            if (detailRow != null) {
                // Invokes the delegates. 
                detailRow(row,column,r);
            }
        }

        private string replaceURLPlaceholders(string url, DataRow row) {
            foreach (DataColumn col in row.Table.Columns) {
                url = url.Replace("%25"+col.ColumnName, "%"+col.ColumnName).Replace("%"+col.ColumnName, row[col].ToString());
            }
            return url;
        }
    }
}
